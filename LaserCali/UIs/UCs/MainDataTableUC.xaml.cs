using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static LaserCali.UIs.UCs.MainDataTableUC;

namespace LaserCali.UIs.UCs
{
    /// <summary>
    /// Interaction logic for MainDataTableUC.xaml
    /// </summary>
    public partial class MainDataTableUC : UserControl
    {
        public ObservableCollection<Author> ListData { get; set; } = new ObservableCollection<Author>();
        public MainDataTableUC()
        {
            InitializeComponent();
            DataContext = this;

            ListData.Add(new Author()
            {
                ID = 101,
                Name = "Mahesh Chand",
                BookTitle = "Graphics Programming with GDI+",
                DOB = new DateTime(1975, 2, 23),
                IsMVP = false
            });
            ListData = LoadCollectionData();
        }

        private ObservableCollection<Author> LoadCollectionData()
        {
            ObservableCollection<Author> authors = new ObservableCollection<Author>();
            authors.Add(new Author()
            {
                ID = 101,
                Name = "Mahesh Chand",
                BookTitle = "Graphics Programming with GDI+",
                DOB = new DateTime(1975, 2, 23),
                IsMVP = false
            });

            authors.Add(new Author()
            {
                ID = 201,
                Name = "Mike Gold",
                BookTitle = "Programming C#",
                DOB = new DateTime(1982, 4, 12),
                IsMVP = true
            });

            authors.Add(new Author()
            {
                ID = 244,
                Name = "Mathew Cochran",
                BookTitle = "LINQ in Vista",
                DOB = new DateTime(1985, 9, 11),
                IsMVP = true
            });

            return authors;
        }


        public class Author
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTime DOB { get; set; }
            public string BookTitle { get; set; }
            public bool IsMVP { get; set; }
        }
    }
}
