using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static ReportMonthResultGenerator.Zendesk;

namespace ReportMonthResultGenerator
{
    internal class ZendeskSearchApiClient : HttpClient
    {
        private static readonly string _zendeskSearchApiUrl = "https://coffeemania.zendesk.com/api/v2/search";
        private static readonly string _countUri = "/count"; // Адрес, где рсположено только число результатов запроса
        private static readonly string _zendeskDateFormat = "yyyy-MM-dd";
        // Лимит количества тикетов, возращаемых от Zendesk SearchAPI за один запрос
        private static readonly int MAX_ZENDESK_SEARCH_RESULTS_COUNT = 1000;

        // Доверять любым сертификатам
        private static readonly RemoteCertificateValidationCallback AcceptAll = (sender, certificate, chain, sslPolicyErrors) => { return true; };

        public ZendeskSearchApiClient()
        {
            var client_id = "a.yakovleva@coffeemania.ru/token";
            var client_secret = "hey4YvrGEpD13BMUw35CPYljBgGlt1mYUp5no2fV";

            ServicePointManager.ServerCertificateValidationCallback = AcceptAll;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{client_id}:{client_secret}"))
            );
            DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Timeout = TimeSpan.FromMinutes(30);
        }


        // Не используется
        // Нет возможности разделить полученные результаты по подразделениям
        public async Task<Dictionary<DateTime, int>> CountTicketsByDay(DateTime begin, DateTime end, bool negativeMoodOnly)
        {
            var result = new Dictionary<DateTime, int>();

            begin = begin.Date;
            end = end.Date;
            int daysInRange = (end - begin).Days + 1; // +1 т.к. включается первый день временного отрезка
            var oneDay = TimeSpan.FromDays(1);



            for (var date = begin; date <= end; date += oneDay)
            {
                var queryString = ConstructSearchQueryUriParameter(date, date, negativeMoodOnly);
                var response = await RetrieveJson<ZendeskSearchResponse>(_zendeskSearchApiUrl + queryString);
                result[date] = response.count;
            }
            return result;
        }


        public async Task<List<ZendeskTicket>> RetrieveTickets(DateTime begin, DateTime end, bool mobileAppOnly = true)
        {
            var query = ConstructSearchQueryUriParameter(begin, end, mobileAppOnly);

            var searchResults = await RetrieveJson<ZendeskSearchResponse>(_zendeskSearchApiUrl + query);
            if (searchResults.count == 0)
            {
                Console.WriteLine($"Загрузка с {begin.ToString("d")} по {end.ToString("d")} завершена. Тикетов не найдено");

                return new List<ZendeskTicket>();
            }
            if (searchResults.count > MAX_ZENDESK_SEARCH_RESULTS_COUNT)
            {
                // Предполагается, что тикетов достаточно мало для того, чтобы получить все тикеты каждого ресторана за день за один запрос
                if (begin.Date == end.Date)
                {

                    if (!DepartmentInfoProvided)
                    {
                        string errMsg = $"За {begin.ToString("d")} было создано {searchResults.count} тикетов. Прогамма не может обработать более {MAX_ZENDESK_SEARCH_RESULTS_COUNT} тикетов за день.";
                        //throw new ZendeskException(errMsg);
                        Console.WriteLine(errMsg);
                        return new List<ZendeskTicket>();
                    }
                    
                    return await RetrieveTicketsAllDepartments(begin.Date, mobileAppOnly);
                    
                }

                // Разбитие промежутка пополам в надежде получить меньше тикетов за запрос
                TimeSpan halfDifferenceOfDays = TimeSpan.FromDays((end - begin).Days / 2);
                TimeSpan oneDay = TimeSpan.FromDays(1);
                // А ещё тикетов достаточно мало, чтобы не вызвать переполнение стека
                var earlierIntervalTask = RetrieveTickets(begin, begin + halfDifferenceOfDays, mobileAppOnly);
                var laterIntervalTask = RetrieveTickets(begin + halfDifferenceOfDays + oneDay, end, mobileAppOnly);
                var earlierIntervalTickets = await earlierIntervalTask;
                var laterIntervalTickets = await laterIntervalTask;

                earlierIntervalTickets.AddRange(laterIntervalTickets);
                return earlierIntervalTickets;
            }
            else
            {
                var result = await FillTicketListFromResponse(searchResults);
                Console.WriteLine($"Загрузка с {begin.ToString("d")} по {end.ToString("d")} завершена. Получего тикетов: {result.Count}");
                return result;
            }
        }

        private Dictionary<int, string> _departments = null;
        public Dictionary<int, string> Departments { set => _departments = value; get => _departments; }
        private bool DepartmentInfoProvided => _departments != null;

        private async Task<List<ZendeskTicket>> RetrieveTicketsAllDepartments(DateTime date, bool mobileAppOnly)
        {
            List<Task<List<ZendeskTicket>>> retriveByDepTasks = new List<Task<List<ZendeskTicket>>>();
            foreach (var dep in Departments)
            {
                var task = RetriveTicketsByDep(date, departmentNumber: dep.Key, name: dep.Value, mobileAppOnly: mobileAppOnly);
                retriveByDepTasks.Add(task);
            }
            var otherDepsTask = RetriveTicketsOtherDeps(date, exclude: Departments.Keys, mobileAppOnly);
            retriveByDepTasks.Add(otherDepsTask);
            await Task.WhenAll(retriveByDepTasks);
            List<ZendeskTicket> result = new List<ZendeskTicket>();
            foreach(var task in retriveByDepTasks)
            {
                result.AddRange(task.Result);
            }
            return result;
        }


        private async Task<List<ZendeskTicket>> RetriveTicketsByDep(DateTime date, int departmentNumber, string name, bool mobileAppOnly)
        {
            var query = ConstructSearchQueryUriParameter(date, date, mobileAppOnly, departmentNumber);
            var searchResults = await RetrieveJson<ZendeskSearchResponse>(_zendeskSearchApiUrl + query);
            if (searchResults.count > MAX_ZENDESK_SEARCH_RESULTS_COUNT)
            {

                string errMsg = $"За {date.ToString("d")} в ресторане {departmentNumber}:{name} было создано {searchResults.count} тикетов. Прогамма не может обработать более {MAX_ZENDESK_SEARCH_RESULTS_COUNT} тикетов за день.";
                //throw new ZendeskException(errMsg);
                Console.WriteLine(errMsg);
                return new List<ZendeskTicket>();
            }
            else
            {
                var result = await FillTicketListFromResponse(searchResults);
                Console.WriteLine($"Загрузка за {date.ToString("d")} ресторана {departmentNumber}:{name} завершена. Получего тикетов: {result.Count}");
                return result;
            }
        }

        private async Task<List<ZendeskTicket>> RetriveTicketsOtherDeps(DateTime date, IEnumerable<int> exclude, bool mobileAppOnly)
        {
            var query = ConstructSearchQueryUriParameter(date, date, mobileAppOnly, exclude);
            var searchResults = await RetrieveJson<ZendeskSearchResponse>(_zendeskSearchApiUrl + query);
            if (searchResults.count > MAX_ZENDESK_SEARCH_RESULTS_COUNT)
            {

                string errMsg = $"За {date.ToString("d")} в остальных ресторанах было создано {searchResults.count} тикетов. Прогамма не может обработать более {MAX_ZENDESK_SEARCH_RESULTS_COUNT} тикетов за день.";
                //throw new ZendeskException(errMsg);
                Console.WriteLine(errMsg);
                return new List<ZendeskTicket>();
            }
            else
            {
                var result = await FillTicketListFromResponse(searchResults);
                Console.WriteLine($"Загрузка за {date.ToString("d")} остальных ресторана завершена. Получего тикетов: {result.Count}");
                return result;
            }
        }

        private static readonly int MAX_RETRIVE_JSON_TRY_COUNT = 100;
        private static readonly TimeSpan SEARCH_API_TIMEOUT = TimeSpan.FromSeconds(1);
        bool firstTry = true; // За этой переменной будут гнаться, но впринципе ничего страшного, просто пару лишних строк в логе
        private async Task<T> RetrieveJson<T>(string url)
        {
            T results = default;
            bool retrieved = false;
            do
            {
                var response = await GetAsync(url);
                string resultsJson;
                using (var stream = new System.IO.StreamReader(await response.Content.ReadAsStreamAsync()))
                {
                    resultsJson = stream.ReadToEnd();
                    try
                    {
                        results = JsonConvert.DeserializeObject<T>(resultsJson);
                        retrieved = true;
                    }
                    catch
                    {
                        if (firstTry)
                        {
                            Console.WriteLine("Перегрузочка. Ждём...");
                            firstTry = false;
                        }
                        retrieved = false;
                        await Task.Delay(SEARCH_API_TIMEOUT);
                    }
                }

            } while (!retrieved);
            firstTry = true;
            return results;
        }

        // Используется для заполнения списка тикетами ТОЛЬКО когда их количество на запрос не превышает лимита 
        private async Task<List<ZendeskTicket>> FillTicketListFromResponse(ZendeskSearchResponse searchResponse)
        {
            var resultingList = new List<ZendeskTicket>(searchResponse.results);
            while (searchResponse.next_page != null)
            {
                searchResponse = await RetrieveJson<ZendeskSearchResponse>(searchResponse.next_page);
                resultingList.AddRange(searchResponse.results);
            }
            return resultingList;
        }

        // Формирует uri параметр запроса, получающего тикеты созданные через мобильное приложение в указанный промежуток времени
        private string ConstructSearchQueryUriParameter(DateTime begin, DateTime end, bool mobileAppOnly, int depId = -1)
        {
            // Правила отбора тикетов, учитывающихся в статистике, смотри в ZendeskExplore, а конкретно -- вычисляемый аттрибут "Средняя Сайт + МП"
            // Тег api должен присутствовать
            string apiTag = "api";
            // "Мерцающий" тег присутствует, если id подразделения указан
            // Пока не используется
            string flickeringSpecificDepartmentTag = (depId == -1) ? "*" : $"{depId}";

            string isFromMobileAppFilter = mobileAppOnly ? $@"tags:""{apiTag} {flickeringSpecificDepartmentTag}""" : $"tags:{flickeringSpecificDepartmentTag}";

            // Теги cloudtips и netmonet должны отсутствовать
            string isNotFromThirdPartyFilter = mobileAppOnly ? "-tags:cloudtips -tags:netmonet" : "";

            // Тикеты из других источников могут иметь пустой рейтинг (нужно для вычисления OnlineStoreNegative)
            string ratingNotEmptyFilter = mobileAppOnly ? $"custom_field_{FieldIdOf.Rating}:*" : "";



            // Не используется (пока)
            string negativeMoodFilter = $@"custom_field_{FieldIdOf.Mood}:{Moods.Negative}";

            return "?query=" + Uri.EscapeDataString(
                $"type:ticket " +
                $"{isFromMobileAppFilter} {isNotFromThirdPartyFilter} {ratingNotEmptyFilter} " +
                $"created_at>={begin.ToString(_zendeskDateFormat)} created_at<={end.ToString(_zendeskDateFormat)}"
                );
        }

        private string ConstructSearchQueryUriParameter(DateTime begin, DateTime end, bool mobileAppOnly, IEnumerable<int> excludeDeps)
        {
            // Правила отбора тикетов, учитывающихся в статистике, смотри в ZendeskExplore, а конкретно -- вычисляемый аттрибут "Средняя Сайт + МП"
            // Тег api должен присутствовать
            string apiTag = "api";
            // "Мерцающий" тег присутствует, если id подразделения указан
            // Пока не используется
            string excludeDepartmentsTags = (excludeDeps.Any()) ? String.Concat(excludeDeps.Select(depNum => " -tags:" + depNum)) : "";

            string isFromMobileAppFilter = mobileAppOnly ? $@"tags:{apiTag}" : "";

            // Теги cloudtips и netmonet должны отсутствовать
            string isNotFromThirdPartyFilter = mobileAppOnly ? "-tags:cloudtips -tags:netmonet ": "";

            // Тикеты из других источников могут иметь пустой рейтинг (нужно для вычисления OnlineStoreNegative)
            string ratingNotEmptyFilter = mobileAppOnly ? $"custom_field_{FieldIdOf.Rating}:*" : "";



            // Не используется (пока)
            string negativeMoodFilter = $@"custom_field_{FieldIdOf.Mood}:{Moods.Negative}";

            return "?query=" + Uri.EscapeDataString(
                $"type:ticket " +
                $"{isFromMobileAppFilter} {isNotFromThirdPartyFilter} {ratingNotEmptyFilter} {excludeDepartmentsTags} " +
                $"created_at>={begin.ToString(_zendeskDateFormat)} created_at<={end.ToString(_zendeskDateFormat)}"
                );
        }

    }

    // Содержит объекты, в которые парсятся JSON-ответы от Zendesk
    static class Zendesk
    {

        // ID кастомных полей в тикектах зендеска
        public static class FieldIdOf
        {
            public static long Department = 360016140737;
            public static long Rating     = 360017986557;
            public static long Mood       = 360016048758;
            public static long Reason     = 360016046938;
        }

        public static class Reasons
        {
            public static bool IsNegative(string reason)
            {
                return ReasonNegativList.Contains(reason);

                switch (reason)
                {
                    case DishQualityForeignObject: return true;
                    case DishQualityFood: return true;
                    case DishQualityBeverage: return true;
                    case DishQualityPoisoning: return true;
                    case RemoteAssembly: return true;
                    case DeliveryAmbigousStatus: return true;
                    case RemotePickup: return true;
                    case RemotePackaging: return true;
                    case DeliverySpeedOurFast: return true;
                    case RemoteAmbigousStatusCourierError: return true;
                    case DeliverySpeedYandexSlow: return true;
                    case DeliverySpeedYandexFast: return true;
                    case DeliveryService: return true;
                    case RemoteCourierService: return true;
                    case DeliverySpeedOurSlow: return true;
                    default: return false;
                }
            }

            public const string dp1 = "дистанционные_продажи__скорость_доставки__долгая__диспетчеризация__долгий_поиск_курьера_яндекс";
            public const string dp2 = "дистанционные_продажи__скорость_доставки__долгая__диспетчеризация__долгий_поиск_курьера_вк";
            public const string dp3 = "дистанционные_продажи__скорость_доставки__долгая__диспетчеризация__курьер_долго_ехал_до_ресторана_после_назначения";
            public const string dp4 = "дистанционные_продажи__скорость_доставки__долгая__диспетчеризация__курьер_ждал_приготовления_2ого_заказа";
            public const string dp5 = "дистанционные_продажи__скорость_доставки__долгая__диспетчеризация__от_2х_заказов_в_1_руки";
            public const string dp6 = "дистанционные_продажи__скорость_доставки__долгая__работа_оператора__оператор_перепутал_заказы";
            public const string dp7 = "дистанционные_продажи__скорость_доставки__долгая__работа_оператора__споз_не_проверил_номер_дома__кв_и_другие_данные_в_строке_номер_гостя";
            public const string dp8 = "дистанционные_продажи__скорость_доставки__долгая__работа_оператора__оператор_забыл_направить_на_кухню";
            public const string dp9 = "дистанционные_продажи__скорость_доставки__долгая__кухня__долго_готовили_блюдо";
            public const string dp10 = "дистанционные_продажи__скорость_доставки__долгая__наша_доставка__дорожная_обстановка";
            public const string dp11 = "дистанционные_продажи__скорость_доставки__долгая__яндекс_и_прочие_сервисы_доставки";
            public const string dp12 = "дистанционные_продажи__скорость_доставки__быстрая__наша_доставка";
            public const string dp13 = "дистанционные_продажи__скорость_доставки__быстрая__яндекс_и_прочие_сервисы_доставки";
            public const string dp14 = "дистанционные_продажи__качество_курьерской_службы__наша_доставка__хамство";
            public const string dp15 = "дистанционные_продажи__качество_курьерской_службы__наша_доставка__курьер_перепутал_заказы";
            public const string dp16 = "дистанционные_продажи__качество_курьерской_службы__наша_доставка__аккуратность_курьера";
            public const string dp17 = "дистанционные_продажи__качество_курьерской_службы__яндекс_и_прочие_сервисы_доставки__хамство";
            //"дистанционные_продажи__коррекция_заказа_по_инициативе_гостя";
            public const string dp18 = "дистанционные_продажи__неясен_статус_заказа__ошибка_ресторана";
            public const string dp19 = "дистанционные_продажи__неясен_статус_заказа__ошибка_курьера";
            //"дистанционные_продажи__неясен_статус_заказа__нейтральный_вопрос_гостя";
            //"дистанционные_продажи__неясен_статус_заказа__не_доставили_по_вине_гостя";
            public const string dp20 = "дистанционные_продажи__качество_сборки_заказа";
            public const string dp21 = "дистанционные_продажи__проблема_с_самовывозом";
            //"дистанционные_продажи__яндекс/деливери_еда__проблемы_у_агрегаторов_";
            public const string dp22 = "дистанционные_продажи__проблема_с_упаковкой";
            //"дистанционные_продажи__просьба_отменить_заказ";
            //"дистанционные_продажи__гость_не_разобрался";

            public static List<string> ReasonNegativList = new List<string>() { dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9, dp10 , 
                dp11, dp12, dp13, dp14, dp15, dp16, dp17, dp18, dp19, dp20, dp21, dp22,
                DishQualityForeignObject, DishQualityFood, DishQualityBeverage, DishQualityPoisoning};

            public const string DeliverySpeedOurSlow = "доставка__скорость_доставки";                                                       //Дистанционные продажи::Скорость доставки: долгая::Наша доставка
            public const string DeliverySpeedYandexSlow = "доставка__скорость_доставки__долгая_яндекс";                                     //Дистанционные продажи::Скорость доставки: долгая::Яндекс и прочие сервисы доставки
            public const string DeliverySpeedOurFast = "доставка__скорость_доставки__быстрая_наша";                                         //Дистанционные продажи::Скорость доставки: быстрая::Наша доставка
            public const string DeliverySpeedYandexFast = "доставка__скорость_доставки__быстрая_яндекс";                                    //Дистанционные продажи::Скорость доставки: быстрая:: Яндекс и прочие сервисы доставки
            public const string DeliveryService = "доставка__сервис_курьерская_служба";                                                     //Дистанционные продажи::Сервис курьерской службы::Наша доставка
            public const string RemoteCourierService = "дистанционные_продажи__сервис_курьерской_службы__яндекс_и_прочие_сервисы_доставки"; //Дистанционные продажи::Сервис курьерской службы::Яндекс и прочие сервисы доставки
            public const string DeliveruGuestInitiatedCorrection = "доставка__коррекция_заказа_по_инициативе_гостя";                        //Дистанционные продажи::Коррекция заказа по инициативе гостя
            public const string DeliveryAmbigousStatus = "доставка__неясен_статус_заказа";                                                  //Дистанционные продажи::Неясен статус заказа::Ошибка ресторана
            public const string RemoteAmbigousStatusCourierError = "дистанционные_продажи__неясен_статус_заказа__ошибка_курьера";           //Дистанционные продажи::Неясен статус заказа::Ошибка курьера
            public const string RemoteAmbigousOrNeutral = "дистанционные_продажи__неясен_статус_заказа__нейтральный_вопрос_гостя";          //Дистанционные продажи::Неясен статус заказа::Нейтральный вопрос Гостя
            public const string RemoteAssembly = "дистанционные_продажи__качество_сборки_заказа";                                           //Дистанционные продажи::Качество сборки заказа
            public const string RemotePickup = "дистанционные_продажи__проблема_с_самовывозом";                                             //Дистанционные продажи::Проблема с самовывозом
            public const string RemoteAggregatorTrouble = "дистанционные_продажи__яндекс/деливери_еда_проблемы_у_агрегатора_";              //Дистанционные продажи::Яндекс/Деливери еда(проблемы у Агрегатора)
            public const string RemotePackaging = "дистанционные_продажи__проблема_с_упаковкой";                                            //Дистанционные продажи::Проблема с упаковкой
            public const string RemoteCancelOrder = "дистанционные_продажи__просьба_отменить_заказ";                                        //Дистанционные продажи::Просьба отменить заказ
            public const string RemoteNotUnderstood = "дистанционные_продажи__гость_не_разобрался";                                         //Дистанционные продажи::Гость не разобрался
            public const string LoyaltyApplication = "программа_лояльности__заявка_на_выпуск_пластиковой_карты";                            //Программа лояльности::Заявка на выпуск пластиковой карты
            public const string LoyaltyUnclearTerms = "программа_лояльности__неясны_условия_пл";                                            //Программа лояльности::Неясны условия ПЛ
            public const string LoyaltyTechnichalIssues = "программа_лояльности__технические_проблемы_пл";                                  //Программа лояльности::Технические проблемы ПЛ
            public const string LoyaltyUnsatisfyingTerms = "программа_лояльности__недовольство_условиями_пл";                               //Программа лояльности::Недовольство условиями ПЛ
            public const string Menu = "состав_меню";                                                                                       //Ассортимент меню
            public const string DishQualityFood = "качество_блюд__качество_еды";                                                            //Качество еды::Качество Блюд
            public const string DishQualityBeverage = "качество_блюд__качество_напитков";                                                   //Качество еды::Качество напитков
            public const string DishQualityDessert = "качество_блюд__качество_десертов";                                                    //Качество еды::Качество десертов
            public const string DishQualityForeignObject = "качество_блюд__инородные_предметы_в_еде";                                       //Качество еды::Инородные предметы в еде
            public const string DishQualityPoisoning = "качество_блюд__отравление";                                                         //Качество еды::Отравление
            public const string Cleanliness = "ресторан._чистота_и_состояние_";                                                             //Рестораны оффлайн::Ресторан. Чистота и состояние 
            public const string Reservation = "бронирование";                                                                               //Рестораны оффлайн::Обслуживание оффлайн::Бронирование
            public const string StaffDemeanorAndApperance = "стандарты_внешнего_вида_и_поведения_сотрудников";                              //Рестораны оффлайн::Обслуживание оффлайн::Стандарты внешнего вида и поведения сотрудников
            public const string Welcoming = "встреча_и_размещение_гостей";                                                                  //Рестораны оффлайн::Обслуживание оффлайн::Встреча и размещение гостей
            public const string ServiceCheckout = "обслуживание__расчет";                                                                   //Рестораны оффлайн::Обслуживание оффлайн::Расчет
            public const string ServiceServingFoodBeverageDessert = "обслуживание__подача_напитков__блюд__десертов";                        //Рестораны оффлайн::Обслуживание: оффлайн:Подача напитков, блюд, десертов
            public const string ServiceWeclome = "обслуживание__приветствие_и_прием_заказа";                                                //Рестораны оффлайн::Обслуживание оффлайн::Приветствие и прием заказа
            public const string Suggestion = "пожелания_по_улучшениям";                                                                     //Пожелания по улучшениям
            public const string ReturnRequest = "запрос_информации_о_возврате_денег";                                                       //Запрос информации о возврате денег
            public const string FiscalRequest = "запрос_документов__фискального_чека_";                                                     //Запрос документов (фискального чека)
            public const string OnlineSite = "вопрос_по_интернет_витринам__им__вопрос_по_сайту";                                            //Вопрос по интернет витринам::Сайт: ошибка или вопрос
            public const string OnlineMobileApp = "вопрос_по_интернет_витринам__им__вопрос_по_мп";                                          //Вопрос по интернет витринам::МП: ошибка или вопрос
            public const string OnlinePreorder = "вопрос_по_интернет_витринам__им__вопрос_по_предзаказу";                                   //Вопрос по интернет витринам::ИМ: Вопрос по Спец. продуктам (предзаказ, кетйтеринг, кулинария)
            public const string OnlineWrongAddress = "вопрос_по_интернет_витринам__им__гость_ошибся_с_адресом_доставки_в_мп";               //Вопрос по интернет витринам::ИМ: Гость ошибся адресом доставки/рестораном для самовывоза
            public const string Accident = "проишествие";                                                                                   //Проишествие
            public const string CakeBuro = "cake_buro";                                                                                     //Cake Buro
            public const string Catering = "вопрос_по_кейтерингу";                                                                          //Вопрос по кейтерингу
            public const string AllGood = "все_хорошо__просто_благодарность.";                                                              //Все хорошо, просто благодарность.
            public const string Unknown = "не_установлено__не_удалось_связаться_для_уточнения_оценки_без_комментариев";                     //Не установлено::Не удалось связаться для уточнения оценки без комментариев
            public const string Intenal = "внутреннее_поручение";                                                                           //Внутреннее поручение
            public const string Other = "другие_вопросы";                                                                                   //Другие вопросы
            public const string QR = "отзывы_с_qr_кода_";                                                                                   //Отзывы с QR кода 
            public const string PreparationQuality = "качество_заготовки";                                                                  //Качество заготовки
            public const string Mailing = "проблема_с_отпиской_от_рассылки/смена/удаление_email";                                           //Проблема с отпиской от рассылки/смена/удаление email
            public const string NotPresent = null;

        }

        // Возможные значения поля Mood (тональность)
        public static class Moods
        {
            public static readonly string Positive = "позитивная"; // 😀 Позитивная 
            public static readonly string Neutral = "нейтральная"; // 😐 Нейтральная
            public static readonly string Negative = "негативная"; // ☹️ Негативная
            public static readonly string Suggestion = "тон_предложение"; // 💡Предложение
            public static readonly string Question = "вопрос"; // Без тональности
            public static readonly string NotPresent = null;
        }

        [Serializable]
        public class IMTicketFields
        {
            [Serializable]
            public class CutsomTicketField
            {
                public long id { get; set; }
                public string title { get; set; }
                public long position { get; set; }
            }
            public List<CutsomTicketField> ticket_fields { get; set; }
        }
        [Serializable]
        public class ZendeskSearchCount
        {
            public int count { get; set; }
        }

        [Serializable]
        public class ZendeskSearchResponse
        {
            public List<ZendeskTicket> results { get; set; }
            public string next_page { get; set; }
            public string previous_page { get; set; }
            public int count { get; set; }
        }
        [Serializable]
        public class Via
        {
            public string channel;
            public object source;
        }
        [Serializable]
        public class ZendeskTicket
        {
            public string url;
            public long? id;
            public long? external_id;
            public Via via;
            public DateTime? created_at;
            public DateTime? updated_at;
            public string type;
            public string subject;
            public string raw_subject;
            public string description;
            public string priority;
            public string status;
            public object recipient;
            public long? requester_id;
            public long? submitter_id;
            public long? assignee_id;
            public long? organization_id;
            public long? group_id;
            public object collaborator_ids;
            public object follower_ids;
            public object email_cc_ids;
            public object forum_topic_id;
            public object problem_id;
            public bool? has_incidents;
            public bool? is_public;
            public object due_at;
            public List<string> tags;
            public List<IdValue> custom_fields;
            public SatisfactionRating satisfaction_rating;
            public List<object> sharing_agreement_ids;
            public List<IdValue> fields;
            public List<object> followup_ids;
            public long? ticket_form_id;
            public long? brand_id;
            public bool? allow_channelback;
            public bool? allow_attachments;
        }

        [Serializable]
        public class SatisfactionRating
        {
            public object score;
        }
        [Serializable]
        public class IdValue
        {
            public long id;
            public object value;
        }

        [Serializable]
        internal class ZendeskException : Exception
        {
            public ZendeskException()
            {
            }

            public ZendeskException(string message) : base(message)
            {
            }

            public ZendeskException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected ZendeskException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

    }
}

