using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    public class ProductivityBase : CalcBase
    {
        // ToDo - #HARDCODE  
        
        public static Dictionary<Type, List<int>> WorkerCats = new Dictionary<Type, List<int>>()
        {
            {typeof(ProductivBarista), new List<int>(){ 5 } },  // 5 = barista        
            {typeof(ProductivSeller), new List<int>(){ 6 } },  //      6 = продавец
            {typeof(ProductivCook), new List<int>(){ 2, 4, 8 } },  //     2 = повар; 4 = старший повар; 8 = су-шеф по кухне
        };
        // ToDo #HARDCODE here - категории товаров для расчета продуктивнос{ 15, 16, 41, 57, 58 };
        public static Dictionary<Type, List<int>> GoodsCats = new Dictionary<Type, List<int>>()
        {
            {typeof(ProductivBarista), new List<int>(){ 1, 3, 38 } }, // 
            {typeof(ProductivSeller), new List<int>(){ 4, 5, 7, 17 ,18, 19, 20, 21, 22, 32, 60 } }, // 11.08.2021: добавлено 60
            //{typeof(ProductivCook), new List<int>(){ 9, 10, 11, 13, 14, 15, 16, 23, 27, 29, 30, 31, 33, 34, 35, 57 } }, // 
            ////                                       9, 10, 11, 13, 14, 15, 16, 23, 27, 29, 30, 31, 33, 34, 35, 57
        };
        // ToDo #HARDCODE here - категории товаров для расчета продуктивнос{ 15, 16, 41, 57, 58 };
        public static Dictionary<Type, List<int>> GoodsCatsExclude = new Dictionary<Type, List<int>>()
        {
            //{typeof(ProductivCook), new List<int>(){ 1, 3, 4, 5, 6, 8, 17, 18, 21, 22, 26, 34, 37, 38, 99 } }, 
            {typeof(ProductivCook), new List<int>(){ 1, 2, 3, 4, 5, 7, 12, 17, 18, 19, 20, 21, 22, 24, 26, 32, 37, 38, 39, 99, 101 } },
        };
        // ToDo #HARDCODE here - количество в чеке, при превышении которого считать товар весовым (даже если он таким не отмечен)
        public static int VesCountLimit = 100;
        // ToDo #HARDCODE here - стандартная порция, если не указан коэффициент
        public static int DefaultPortionRatio = 150;


        public int GetTypeId()
        {
            return TypeId;
        }
        public override List<ReportDayResult> Calc(DateTime day)
        {
            return null;
        }

    }
}
