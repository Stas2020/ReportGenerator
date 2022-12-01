using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace ReportMonthResultGenerator
{
    public static class MOZGIntegration
    {
        public class RestState
        {
            public int RestNum;
            public bool IsAero;
            public decimal GuestOnCheck;
            public int ChecksCount;
            public int GuestCount;
            public decimal DishesCount = 0;
            public decimal DrinksCount = 0;
            //public double DishesCountWoPK = 0;
            //public double DrinksCountWoPK = 0;
            public decimal DishesOnCheck { get { return ChecksCount != 0 ? (DishesCount / (decimal)ChecksCount) : 0; } }
            public decimal DrinksOnCheck { get { return ChecksCount != 0 ? (DrinksCount / (decimal)ChecksCount) : 0; } }
            public decimal DishesOnGuest { get { return GuestOnCheck != 0 ? (DishesOnCheck / (decimal)GuestOnCheck) : 0; } }
            public decimal DrinksOnGuest { get { return GuestOnCheck != 0 ? (DrinksOnCheck / (decimal)GuestOnCheck) : 0; } }
        }

        public static Dictionary<DateTime, List<RestState>> RestStatistics = new Dictionary<DateTime, List<RestState>>();

        public static RestState GetRestData(DateTime _day, int _restNum)
        {
            if (!RestStatistics.ContainsKey(_day))
                CalculateDay(_day);

            return RestStatistics[_day].FirstOrDefault(_stat => _stat.RestNum == _restNum);
        }

        public static void CalculateDay(DateTime day)
        {
            // set these values correctly for your database server
            var builders = new List<MySqlConnectionStringBuilder>()
            {
            new MySqlConnectionStringBuilder{
                Server = "192.168.78.81",//CITY
                UserID = "dashboard",
                Password = "GfR7k4rf",
                Database = "mozg_938",
                ConnectionTimeout = 1800,
                DefaultCommandTimeout = 1800,
            },
            new MySqlConnectionStringBuilder{
                Server = "192.168.78.83",//AERO
                UserID = "dashboard",
                Password = "GfR7k4rf",
                Database = "mozg_1027",
                ConnectionTimeout = 1800,
                DefaultCommandTimeout = 1800,
            }
            };


            //int TEST = 0;
            //DateTime TESTtime0 = DateTime.Now;
            //var TESTspan = (DateTime.Now - TESTtime0).TotalSeconds;


            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();


            foreach (var builder in builders)
            {
                List<RestState> BuilderStat = new List<RestState>();


                using (MySqlConnection connection = new MySqlConnection(builder.ConnectionString))
                {
                    connection.Open();// Async();
                                      // create a DB command and set the SQL statement with parameters
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        //DateTime day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);


                        command.CommandText = $"DROP TEMPORARY TABLE if exists tmp_order;";
                        if (builder.Database == "mozg_938")
                            command.CommandText += "CREATE TEMPORARY TABLE tmp_hall " +
                            $"SELECT hall_id, hall_rest_id FROM {builder.Database}.hall " +
                            $"inner join {builder.Database}.hllg on hllg_id = hall_hllg_id " +
                            "where hllg_desc like'%Зал%' " +
                            "or hllg_desc like'%Летняя терраса%';";

                        command.CommandText += $"SELECT count(*) as check_count,sum(order_guest) as guest_count,order_rest_id " +
                        $"FROM {builder.Database}.order ";
                        //$"where order_date between '{day:yyyy-MM-dd}' and '{day:yyyy-MM-dd}' " +
                        if (builder.Database == "mozg_938")
                            command.CommandText += $" inner join tmp_hall on tmp_hall.hall_id = {builder.Database}.order.order_hall_id and tmp_hall.hall_rest_id = {builder.Database}.order.order_rest_id ";

                        command.CommandText += $"where order_date = '{day:yyyy-MM-dd}' ";

                        //if (builder.Database == "mozg_938")
                        //    command.CommandText += $" and order_hall_id =any(select hall_id from tmp_hall)";

                        command.CommandText += $"group by order_rest_id;" +
                            $"DROP TEMPORARY TABLE if exists tmp_hall;";
                        //command.Parameters.AddWithValue("@OrderId", orderId);


                        // execute the command and read the results
                        using (var reader = command.ExecuteReader())//command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var rest_id = GetRealRestNum(reader.GetInt32("order_rest_id"));
                                var check_count = reader.GetInt32("check_count");
                                var guest_count = reader.GetInt32("guest_count");

                                var dep = DepList.FirstOrDefault(_dep => _dep.Number == rest_id && _dep.Enabled);
                                if (dep == null)
                                    continue;
                                if (!((dep.Place.ToLower() == "город" && builder.Database == "mozg_938") || (dep.Place.ToLower() != "город" && builder.Database != "mozg_938")))
                                    continue;

                                var existing = BuilderStat.FirstOrDefault(_stat => _stat.RestNum == rest_id);
                                if (existing == null)
                                {
                                    //RestStatistics[day].Remove(existing);
                                    BuilderStat.Add(new RestState()
                                    {
                                        RestNum = rest_id,
                                        IsAero = builder.Database != "mozg_938",
                                        ChecksCount = check_count,
                                        GuestCount = guest_count,
                                        GuestOnCheck = (decimal)guest_count / (decimal)check_count
                                    });
                                    //if (rest_id == 301)
                                    //    ;
                                }
                            }
                        }
                        //command.Dispose();
                    }

                    //{
                    //    var vesna = RestStatistics[day].FirstOrDefault(_r => _r.RestNum == 302);
                    //    var gum = RestStatistics[day].FirstOrDefault(_r => _r.RestNum == 301);
                    //}
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        //DateTime day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //DateTime day = new DateTime(2022, 05, 01);


                        command.CommandText = $"DROP TEMPORARY TABLE if exists tmp_order;" +
                            $"DROP TEMPORARY TABLE if exists tmp_hall;";

                        if (builder.Database == "mozg_938")
                            command.CommandText += "CREATE TEMPORARY TABLE tmp_hall " +
                            $"SELECT hall_id, hall_rest_id FROM {builder.Database}.hall " +
                            $"inner join {builder.Database}.hllg on hllg_id = hall_hllg_id " +
                            "where hllg_desc like'%Зал%' " +
                            "or hllg_desc like'%Летняя терраса%';";

                        command.CommandText += $"CREATE TEMPORARY TABLE tmp_order " +
                        $"SELECT order_rest_id, order_visit_id " +
                        $"FROM {builder.Database}.order ";

                        if (builder.Database == "mozg_938")
                            command.CommandText += $" inner join tmp_hall on tmp_hall.hall_id = {builder.Database}.order.order_hall_id and tmp_hall.hall_rest_id = {builder.Database}.order.order_rest_id ";

                        command.CommandText += $"where order_date = '{day:yyyy-MM-dd}' ";

                        //if (builder.Database == "mozg_938")
                        //    command.CommandText += $" and order_hall_id =any(select hall_id from tmp_hall)";

                        command.CommandText += $";" +
                        $"SELECT p.pbnd_rest_id, p.pbnd_visit_id, p.pbnd_item_id, p.pbnd_qntt," +
                        $"iDish.item_portion as dishPortion, iDrink.item_portion as drinkPortion " +
                        $"from {builder.Database}.pbnd as p " +
                        $"left join {builder.Database}.item as iDish on iDish.item_id = p.pbnd_item_id and iDish.item_categ_mozg_id in(1,2) " +
                        $"left join {builder.Database}.item as iDrink on iDrink.item_id = p.pbnd_item_id and iDrink.item_categ_mozg_id in(3,4,7,8) " +
                        $"where exists(SELECT o.order_visit_id FROM tmp_order as o where p.pbnd_rest_id = o.order_rest_id and p.pbnd_visit_id = o.order_visit_id); " +
                        $"" +
                        $"" +
                        $"DROP TEMPORARY TABLE if exists tmp_order;" +
                            $"DROP TEMPORARY TABLE if exists tmp_hall;";
                        //command.Parameters.AddWithValue("@OrderId", orderId);


                        // execute the command and read the results
                        using (var reader = command.ExecuteReader())//command.ExecuteReaderAsync())
                        {
                            //TESTspan = (DateTime.Now - TESTtime0).TotalSeconds;
                            while (reader.Read())
                            {
                                //TEST++;

                                var rest_id = GetRealRestNum(reader.GetInt32("pbnd_rest_id"));
                                var visit_id = reader.GetString("pbnd_visit_id");
                                var item_id = reader.GetString("pbnd_item_id");
                                Single quan = !reader.IsDBNull(reader.GetOrdinal("pbnd_qntt")) ? reader.GetFloat("pbnd_qntt") : 0;
                                Single dishPortion = !reader.IsDBNull(reader.GetOrdinal("dishPortion")) ? reader.GetFloat("dishPortion") : 0;
                                Single drinkPortion = !reader.IsDBNull(reader.GetOrdinal("drinkPortion")) ? reader.GetFloat("drinkPortion") : 0;


                                var dep = DepList.FirstOrDefault(_dep => _dep.Number == rest_id && _dep.Enabled);
                                if (dep == null)
                                    continue;
                                if (!((dep.Place.ToLower() == "город" && builder.Database == "mozg_938") || (dep.Place.ToLower() != "город" && builder.Database != "mozg_938")))
                                    continue;

                                if ((dishPortion != 0 || drinkPortion != 0) && quan != 0)
                                {
                                    var existing = BuilderStat.FirstOrDefault(_stat => _stat.RestNum == rest_id);
                                    if (existing != null)
                                    {

                                        //if (rest_id == 301)
                                        //    ;

                                        existing.DishesCount += (decimal)((decimal)quan * (decimal)dishPortion);
                                        existing.DrinksCount += (decimal)((decimal)quan * (decimal)drinkPortion);
                                        //if (dishPortion > 0)
                                        //    existing.DishesCountWoPK += quan;
                                        //else
                                        //    existing.DrinksCountWoPK += quan;
                                    }
                                    else
                                    {
                                        ;// It is not real
                                    }
                                }
                            }
                        }
                        //command.Dispose();
                    }
                    connection.Close();
                }


                if (!RestStatistics.ContainsKey(day))
                    RestStatistics.Add(day, new List<RestState>());

                foreach (var sts in BuilderStat)
                {
                    if (RestStatistics[day].FirstOrDefault(_stat => _stat.RestNum == sts.RestNum) == null)
                        RestStatistics[day].Add(sts);
                }
            }
            {
                var vesna = RestStatistics[day].FirstOrDefault(_r => _r.RestNum == 302);
                var gum = RestStatistics[day].FirstOrDefault(_r => _r.RestNum == 301);
            }

        }
        static int GetRealRestNum(int _num)
        {
            switch (_num)
            {
                case 191:
                    return 190;
                //case 111:
                //    return 121;
                case 123:
                    return 122;
                case 236:
                    return 235;
                case 111:
                    return 114;
                case 104:
                    return 124;
                case 311:
                    return 331;
                case 300:
                    return 242;
                case 231:
                    return 244;
                case 380:
                    return 301;
                case 270:
                    return 302;
                case 205:
                    return 276;
                case 177:
                    return 277;
                case 390:
                    return 278;
                case 295:
                    return 281; // 2022-10-18 add комсомольский
                case 180:
                    return 282; // 2022-10-18 add осенняя

                default:
                    return _num;
            }
        }









        // Key - depnum, value - orders count
        public static Dictionary<int, int> GetIMOrders(DateTime day)
        {
            // set these values correctly for your database server
            var builders = new List<MySqlConnectionStringBuilder>()
            {
            new MySqlConnectionStringBuilder{
                Server = "192.168.78.81",//CITY
                UserID = "dashboard",
                Password = "GfR7k4rf",
                Database = "mozg_938",
                ConnectionTimeout = 1800,
                DefaultCommandTimeout = 1800,
            },
            new MySqlConnectionStringBuilder{
                Server = "192.168.78.83",//AERO
                UserID = "dashboard",
                Password = "GfR7k4rf",
                Database = "mozg_1027",
                ConnectionTimeout = 1800,
                DefaultCommandTimeout = 1800,
            }
            };


            //int TEST = 0;
            //DateTime TESTtime0 = DateTime.Now;
            //var TESTspan = (DateTime.Now - TESTtime0).TotalSeconds;


            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Dictionary<int, int> result = new Dictionary<int, int>();

            foreach (var builder in builders)
            {
                List<RestState> BuilderStat = new List<RestState>();


                using (MySqlConnection connection = new MySqlConnection(builder.ConnectionString))
                {
                    connection.Open();// Async();
                                      // create a DB command and set the SQL statement with parameters
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText += $" DROP TEMPORARY TABLE if exists tmp_hall; " +
                        $" CREATE TEMPORARY TABLE tmp_hall " +
                        $" SELECT hall_id, hall_rest_id " +
                        $" FROM {builder.Database}.hall " +
                        $" inner join {builder.Database}.hllg on hllg_id = hall_hllg_id " +
                        $" where hllg_desc like'%Инернет%' " +
                        $" or hllg_desc like'%Интернет%'; " +
                        $" SELECT order_rest_id, count(*) as check_count " +
                        $" FROM {builder.Database}.order " +
                        $" inner join tmp_hall on tmp_hall.hall_id = {builder.Database}.order.order_hall_id and tmp_hall.hall_rest_id = {builder.Database}.order.order_rest_id " +
                        $" where order_date = '{day:yyyy-MM-dd}' " +
                        $" group by order_rest_id; " +
                        $" DROP TEMPORARY TABLE if exists tmp_hall; ";
                        //command.Parameters.AddWithValue("@OrderId", orderId);


                        // execute the command and read the results
                        using (var reader = command.ExecuteReader())//command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var rest_id = GetRealRestNum(reader.GetInt32("order_rest_id"));
                                var check_count = reader.GetInt32("check_count");

                                var dep = DepList.FirstOrDefault(_dep => _dep.Number == rest_id && _dep.Enabled);
                                if (dep == null)
                                    continue;
                                if (!((dep.Place.ToLower() == "город" && builder.Database == "mozg_938") || (dep.Place.ToLower() != "город" && builder.Database != "mozg_938")))
                                    continue;

                                if (!result.ContainsKey(rest_id))
                                {
                                    result[rest_id] = check_count;
                                }
                            }
                        }
                        //command.Dispose();
                    }


                    connection.Close();
                }



            }
            return result;
        }
    }
}
