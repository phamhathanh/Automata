using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Automata.View
{
    /// <summary>
    /// Interaction logic for CodeChecker.xaml
    /// </summary>
    public partial class CodeChecker : Window
    {
        IEnumerable<string> inputLanguageStrings;

        public CodeChecker()
        {
            InitializeComponent();
        }

        private void btnCheckCode_Click(object sender, RoutedEventArgs e)
        {
            bool checkResult = false;
            this.inputLanguageStrings = txtInputLanguage.Text.Replace(" ", "").Split(',').ToArray();
            List<IEnumerable<string>> U = new List<IEnumerable<string>>();
            U.Add(findRedundancyWithoutEpsilon(inputLanguageStrings, inputLanguageStrings, false));

            while (true)
            {
                IEnumerable<string> Ui = findRedundancyWithoutEpsilon(U[U.Count - 1], inputLanguageStrings).Union(findRedundancyWithoutEpsilon(inputLanguageStrings, U[U.Count - 1]));

                foreach (string kkk in Ui)
                {
                    Console.Write("," + kkk);
                    
                }
                Console.WriteLine("//");

                if (Ui.Count() == 0)
                {
                    checkResult = true;
                    break;
                }

                if (Ui.Contains(""))
                {
                    checkResult = false;
                    break;
                }

                foreach (IEnumerable<string> Uk in U)
                {
                    if (Uk.Count() == Ui.Count() && Uk.Intersect(Ui).Count() == Uk.Count())
                        checkResult = true;
                }
                if (checkResult) break;

                U.Add(Ui);
            }

            if (checkResult)
                txtInputLanguage.Background = Brushes.Green;
            else
                txtInputLanguage.Background = Brushes.Red;
        }

        private List<string> findRedundancyWithoutEpsilon(IEnumerable<string> lang1, IEnumerable<string> lang2, bool emptyString = true)
        {
            List<string> redundancy = new List<string>();

            foreach (string checkString1 in lang1)
            {
                foreach (string checkString2 in lang2)
                {
                    if (checkString2.IndexOf(checkString1) == 0)
                    {
                        if (!emptyString && checkString1 == checkString2)
                            continue;
                        string match = checkString2.Substring(checkString1.Length);
                        if (!redundancy.Contains(match))
                            redundancy.Add(checkString2.Substring(checkString1.Length));
                    }
                }
            }

            return redundancy;
        }
    }
}
