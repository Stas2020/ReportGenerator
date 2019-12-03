using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.Tasks
{
    public  class MenuAllItemsUpdaterTask
    {
        public void Run()
        {
            int drincBottleCatNumber = 91;
            int kitchenCatNumber = 41;
            int stoykaCatNumber = 44;
            int drinkCupCatNumber = 92;
            string CFCConnection = @"Data Source=NewSquare1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=";
            
            var items = (new CFCCommonData.CFCQueries(CFCConnection)).GetCurentMnuAll();

            List<AlohaMenuItemsAll> outItems = items.Select(a =>
            {
                var itm = new AlohaMenuItemsAll()
                {
                    BarCode = a.Number,
                    CategoryId = a.Category.Number,
                    Name= a.LongName
                };
                itm.Weight = a.CategoryItem.Any(b => b.Category.Number == drincBottleCatNumber) ? 4 : 1;
                itm.IsDish = a.CategoryItem.Any(b => b.Category.Number == kitchenCatNumber || b.Category.Number == stoykaCatNumber);
                itm.IsDrink = a.CategoryItem.Any(b => b.Category.Number == drincBottleCatNumber || b.Category.Number == drinkCupCatNumber);
                return itm;
            }).ToList();


            var outCats = items.GroupBy(a=>a.Category.Number).Select(a =>

                new AlohaMenuCatAll()
                {
                    Name = a.FirstOrDefault().Category.Name,
                    Cat = a.Key
                }).Distinct().ToList();


            var comps = (new CFCCommonData.CFCQueries(CFCConnection)).GetCurentComps();
            var outComps = comps.Select(a => new AlohaMenuComps()
            {
                AlohaCompId = a.Number,
                Name = a.Name
            }).ToList();



            var dbMNU = new ReportBaseDataContext();

            foreach (var item in outComps)
            {
                InsertWithCheck(dbMNU.AlohaMenuComps, item, a => a.AlohaCompId == item.AlohaCompId);
            }

            foreach (var cat in outCats)
            {
                if (dbMNU.AlohaMenuCatAll.Any(a => a.Cat == cat.Cat))
                {
                    dbMNU.AlohaMenuCatAll.DeleteAllOnSubmit(dbMNU.AlohaMenuCatAll.Where(a => a.Cat == cat.Cat));
                }
                dbMNU.AlohaMenuCatAll.InsertOnSubmit(cat);

            }

            foreach (var itm in outItems)
            {
                if (dbMNU.AlohaMenuItemsAll.Any(a => a.BarCode == itm.BarCode))
                {
                    dbMNU.AlohaMenuItemsAll.DeleteAllOnSubmit(dbMNU.AlohaMenuItemsAll.Where(a => a.BarCode == itm.BarCode));
                }
                dbMNU.AlohaMenuItemsAll.InsertOnSubmit(itm);
                
            }
            dbMNU.SubmitChanges();
        }
        private void InsertWithCheck<T>(System.Data.Linq.Table<T> table, T item, Func<T,bool> keySelector)
            where T:class
        {
            //foreach (var item in items)
            {
                if (table.Any(keySelector))
                {
                    table.DeleteAllOnSubmit(table.Where(keySelector));
                }
                table.InsertOnSubmit(item);
            }
        }
    }
}
