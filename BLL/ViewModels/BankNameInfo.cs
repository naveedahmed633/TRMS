using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    // This class is just a Big View Model for sending multiple models to the
    // employee CRUD view.
    public class BankNameInfo
    {
        public int Id { get; set; }

        public string BankNameText { get; set; }

        public string action { get; set; }

    }
}
