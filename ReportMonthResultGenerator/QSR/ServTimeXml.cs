using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ReportMonthResultGenerator.QSR
{
    [XmlRoot(ElementName = "ServTime.xml")]
    public  class ServTimeXml
    {
        public ServTimeXml() { }
        [XmlIgnore]
        DateTime DateOfBusiness;

    //    [XmlArray(ElementName = "ServiceTiming")]
        public  List<CServiceTiming> ServiceTiming { set; get; }


    }
    
    public class CServiceTiming
    {
        public CServiceTiming() { }
        public int TransactionNumber { set; get; }
        public int ItemNumber { set; get; }
        public int ItemId { set; get; }
        public int ItemCookTime { set; get; }
        public int OrderFirstDisplayedTime { set; get; }
        public int OrderLastBumpTime { set; get; }
        public int VirtualDisplayId { set; get; }
        public int ServerId { set; get; }
        public DateTime Order_Start_Time { set; get; }
    }



}
