using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImadeModal_Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            bool result;
            ImageServiceModal iModal = new ImageServiceModal("C:\\users\\ranran9991\\Desktop", 120);
            string error = iModal.AddFile("C:\\users\\ranran9991\\Desktop\\lol.jpg", out result);
            System.Console.WriteLine(result); System.Console.WriteLine(error);
            Console.ReadKey();
        }
    }
}
