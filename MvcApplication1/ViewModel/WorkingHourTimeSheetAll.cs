using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

///This view model class has been referred from example created by Marien Monnier at Soft.it. All credits to Marien for this class
namespace WorkingHourTimeSheetAll.Models
{

    public class WorkingHourTimeSheetAllViewModel
    {
        public List<BLL.PdfReports.MonthlyTimeSheetData> data {get; set; }
    }

}