using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator
{
    static class AlWebApi
    {
        static HttpClient client = new HttpClient();

        public static List<DepartStat> GetDepartStat()
        {
           Init();
            var myDepartments = GetMyDepartmentAsync().Result;
            var checkLisrs = GetDepartStatAsync().Result;
            checkLisrs.ForEach(a => a.DepId = myDepartments.Single(b => b.DepId == a.DepId).DepNum);
            return checkLisrs;
        }

        static void Init()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            string path = @"https://s2010/complaints/api/auth/getuser?Login=b.rodriguez&Password=b4e35b7878e2c7e2fafe260da63295ad91dfae2031dadb2ad23db7b602778eb5";
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            HttpResponseMessage response = client.GetAsync(path).Result;

        }




        static async Task<List<MyDepartment>> GetMyDepartmentAsync()
        {
            string path = $"https://s2010/complaints/api/info/getdepartments";
            List<MyDepartment> myDepartments = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                myDepartments = await response.Content.ReadAsAsync<List<MyDepartment>>();
            }
            return myDepartments;
        }

        static async Task<List<DepartStat>> GetDepartStatAsync()
        {
            string path = $"https://s2010/complaints/api/info/GetCheckListReportStats";
            List<DepartStat> myDepartments = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                myDepartments = await response.Content.ReadAsAsync<List<DepartStat>>();
            }
            return myDepartments;
        }
    }

    public class MyDepartment
    {
        public int DepId { get; set; } // мой ID подразделения
        public int DepNum { get; set; } // кофеманский номер подразделения
        public string DepName { get; set; } // Имя подразделения
        public bool isActive { get; set; } // флаг активно подразделение или нет . у Алко подразделений флаг активности - 0 в любом случае
    }

    public class DepartStat
    {
        public string DepName { get; set; } // имя подразделения
        public int DepId { get; set; } // мой внутренний ID подразделения
        public List<CheckListStat> CheckListStats { get; set; } // Лист CheckListStat'ов данного подразделения

    }

    public class CheckListStat
    {
        public string CheckListName { get; set; } // имя чек листа
        public long CheckListId { get; set; } // ID чек листа
        public Stat LastStat { get; set; } // Последний отчет из цепочки отчетов
        public List<Stat> Stats { get; set; } // Лист Stat'ов
    }

    public class Stat
    {
        public DateTime StatDate { get; set; } //дата отчета
        public decimal StatRatio { get; set; } // баллы
        public bool StatCompleted { get; set; } // флаг закончен отчет или нет ( на данный момент не актуален )
    }

    public class GetData<T>
    {
        public GetData(string path)
        { }

        static HttpClient client = new HttpClient();
        async Task<MyDepartment> GetProductAsync(string path)
        {
            MyDepartment product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<MyDepartment>();
            }
            return product;
        }

    }

}
