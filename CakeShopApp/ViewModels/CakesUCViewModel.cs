using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeShopApp.ViewModels
{
    class CakesUCViewModel : BaseViewModel
    {
        public ObservableCollection<int> Categories { get; set; }
        public CakesUCViewModel()
        {

        }
    }
}
