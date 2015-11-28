using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Patterson
    {
        private readonly RegularLanguage language;
        private readonly List<string> element;

        public Patterson(RegularLanguage language)
        {
            this.language = language;
            int elementCount = this.language.Expression.Split('|').ToList().Count;
            List<string> listSplit = this.language.Expression.Substring(1, this.language.Expression.ToString().Length - 2).Split('|').ToList();
            this.element = listSplit;

        }

        public List<string> LeftSplitByElementInCollection(List<string> element, string spitString)
        {
            List<string> collection = new List<string>();
            for (int i = 0; i < element.Count; i++)
            {
                if (element[i].StartsWith(spitString))
                {
                    if (!collection.Contains(SplitString(element[i], spitString)))
                        collection.Add(SplitString(element[i], spitString));
                }
            }
            return collection;
        }

        public List<string> LeftSplitByCollection(List<string> element, List<string> splitCollection)
        {
            List<string> collection = new List<string>();
            foreach (var item in splitCollection)
            {
                AddMemberToCollection(collection, item, element);
            }
            return collection;
        }

        public List<string> MergeTwoCollection(List<string> list1, List<string> list2)
        {
            for (int i = 0; i < list2.Count; i++)
            {
                if (!list1.Contains(list2[i]))
                {
                    list1.Add(list2[i]);
                }
            }
            return list1;
        }

        private void AddMemberToCollection(List<string> collection, string item, List<string> element)
        {
            List<string> ListOfCollection = LeftSplitByElementInCollection(element, item);
            for (int i = 0; i < ListOfCollection.Count; i++)
            {
                if (!collection.Contains(ListOfCollection[i]))
                    collection.Add(ListOfCollection[i]);
            }
        }



        private string SplitString(string p, string splitString)
        {
            if (p == splitString)
                return "Exilon";
            else return p.Substring(splitString.Length);
        }

        public bool IsUniquelyDecodable()
        {
            List<List<string>> listCollection = new List<List<string>>();
            listCollection.Add(new List<string>());
            listCollection[0] = LeftSplitByCollection(element, element);
            if (listCollection[0].Count == 0 || (listCollection[0].Count == 1 && listCollection[0].Contains("Exilon")))
                return true;
            else
            {
                if (listCollection[0].Contains("Exilon"))
                    listCollection[0].RemoveAll(u => u.Equals("Exilon"));
                int i = 1; int j = 1;
                bool output = true;
                while (i == j)
                {
                    listCollection.Add(new List<string>());
                    listCollection[i] = MergeTwoCollection(LeftSplitByCollection(element, listCollection[i - 1]), LeftSplitByCollection(listCollection[i - 1], element));
                    if (listCollection[i].Contains("Exilon"))
                    {
                        output = false;
                        i++;
                    }
                    else
                    {
                        if (listCollection[i].Count == 0 || ExistTwoCollectionEqual(listCollection, i))
                        {
                            output = true;
                            i++;
                        }
                        else
                        {
                            i++;
                            j++;
                        }
                    }

                }

                return output;
            }

        }

        private bool ExistTwoCollectionEqual(List<List<string>> listCollection, int i)
        {
            for (int j = 0; j < i; j++)
            {
                if (TwoCollectionEqual(listCollection[i], listCollection[j]))
                    return true;
            }
            return false;
        }

        private bool TwoCollectionEqual(List<string> list1, List<string> list2)
        {
            for (int i = 0; i < list2.Count; i++)
            {
                if (!list1.Contains(list2[i]))
                    return false;
            }
            return true;
        }
    }
}
