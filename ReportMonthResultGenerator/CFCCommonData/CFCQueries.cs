using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.CFCCommonData
{
    class CFCQueries
    {
        string CityOwnerId = "9DFF9173-BCB4-4F83-AE77-C1DB74E6776A";
        int LastDishBarcodeNum = 933000;
        //string CityOwnerId = "9DFF9173-BCB4-4F83-AE77-C1DB74E6776A";
        string ConnectionString = "";
        public CFCQueries(string _connectionString, string _CityOwnerId)
        {
            this.ConnectionString = _connectionString;
            this.CityOwnerId = _CityOwnerId;
        }

        public List<Item> GetCurentMnuAll()
        {
            var db = new DBCFCDataContext(ConnectionString);            
            return db.Item.Where(a => a.FK_Owner.ToString() == CityOwnerId && a.Number<=LastDishBarcodeNum).ToList();
        }
        public List<Comp> GetCurentComps()
        {
            var db = new DBCFCDataContext(ConnectionString);
            return db.Comp.Where(a => a.FK_Owner.ToString() == CityOwnerId && a.Active.GetValueOrDefault()).ToList();
        }
        /*
        public List<Category> GetCurentMnuAll()
        {
            var db = new DBCFCDataContext(ConnectionString);
            return db.Item.Where(a => a.FK_Owner.ToString() == CityOwnerId && a.Number <= LastDishBarcodeNum).ToList();
        }
        */
    }
}
