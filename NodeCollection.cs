using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WordleSolver
{
    class NodeCollection
    {
        public Node Root;
        string[] LockedLetters;

        public NodeCollection()
        {
            Root = new Node(null);
            LockedLetters = new string[] { null, null, null, null, null, null };
        }

        public void AddDictionary(string path)
        {
            StreamReader reader = new StreamReader(path);

            string line = reader.ReadLine();

            while (line != null)
            {
                line = line.Trim();
                
                if (line.Length == 5)
                {
                    line = "#" + line.ToLower();
                    Root.Add(line);
                }

                line = reader.ReadLine();
            }

            reader.Close();
        }

        //Green Letter Sceanrio. Delete all of the letters at a certain level\slot which are not the specified value.
        public void LockLetter(int level, string value)
        {
            LockedLetters[level] = value;
            LockLetter(level, value, Root);
        }
        
        public void LockLetter(int level, string value, Node search)
        {
            if (search == null)
            {
                return;
            }

            //Go Deeper if we are not on the right level
            if (search.Level < level)
            {
                foreach(Node node in search.Children.Values)
                {
                    LockLetter(level, value, node);
                }
            }

            //If we are at the desired level, remove any nodes which are not the letter
            if (search.Level == level && search.Value != value)
            {
                search.Remove();
            }
        }

        //Grey Letter Scenario. Remove all instances of a letter from the tree
        public void RemoveLetter(string value)
        {
            RemoveLetter(value, Root);
        }

        public void RemoveLetter(string value, Node search)
        {
            //Base Case. No expansion needed if searcb  node is null
            if (search == null)
            {
                return;
            }

            //Base Case. If the search node is the value we are looking for, delete it and as a result all of it's successors
            if (search.Value == value && isNotLocked(search))
            {
                search.Remove();
                return;
            }
            
            //Expand each of the children looking for the value of interest
            foreach (Node node in search.Children.Values)
            {
                RemoveLetter(value, node);
            }
        }

        //Orange Letter Case. Ensure all words have at least once occurance of the letter.
        //                    Remove the letter at the identified slot

        public void FloatLetter(int[] levels, string value)
        {
            //Remove the letter at the orange slots since we know the letter cannot occupy that space
            foreach(int level in levels)
            {
                RemoveLetterAtLevel(level, value, Root);
            }

            //Search the tree to verify there are at least a levels.Count number of occurances of value in the word. If not remove
            MustHaveLetter(value, levels.Length, Root, 0);
        }

        //Ensure that each word in the tree has a certain number of instances of the letter. If not remove the word.
        public void MustHaveLetter (string value, int occurances, Node search, int found)
        {
            //Base Case: Node we are serching at is null
            if (search == null)
            {
                return;
            }

            //Base Case: We have found an instance of the letter
            if (search.Value == value && isNotLocked(search))
            {
                found++;
            }

            //Base Case: If we have found enough instances of the letter, we can stop searching
            if (found >= occurances)
            {
                return;
            }

            //Base Case: We are at the bottom level and the number of occurances is not sufficient. We have to remove the wokr
            if (search.Level == 5 && found < occurances)
            {
                search.Remove();
                return;
            }

            //Continue searching the tree until we find enough occurances
            foreach (Node node in search.Children.Values)
            {
                MustHaveLetter(value, occurances, node, found);
            }
        }

        //Function to Remove a Letter at a specific slot only. Importannt for resolving orange squares
        public void RemoveLetterAtLevel(int level, string value, Node search)
        {
            //Base Case: The node we're searching for is null, nothing to expand
            if (search == null)
            {
                return;
            }

            //Base Case: We are past the level we are searching for
            if (search.Level > level)
            {
                return;
            }

            //Base Case: We are on the correct level, and we have found the value we are searching for
            if (search.Level + 1 == level && search.Children.ContainsKey(value) && search.Children[value] != null && isNotLocked(search.Children[value]))
            {
                search.Children[value].Remove();
                return;
            }

            //Search the next level down, but don't go past the desired level
            if (search.Level + 1 < level)
            {
                foreach(Node node in search.Children.Values)
                {
                    RemoveLetterAtLevel(level, value, node);
                }
            }
        }

        public bool isNotLocked(Node node)
        {
            return LockedLetters[node.Level] != node.Value;
        }

        //Function to find the most likely wordle prediction given the state of the tree
        public string MostLikely()
        {
            return MostLikely(Root);
        }

        public string MostLikely(Node search)
        {
            int maxSuccessors = int.MinValue;
            Node maxNode = null;

            //Find which of the children has the highest number of children
            foreach (Node node in search.Children.Values)
            {
                if (node != null && node.TotalSuccessors > maxSuccessors)
                {
                    maxSuccessors = node.TotalSuccessors;
                    maxNode = node;
                }
            }

            //Base Case: We are at the bottom of the tree and need to return the most likely word
            if (maxNode.Level == 5)
            {
                return maxNode.Word;
            }
            else
            {
                return MostLikely(maxNode);
            }
        }

        public string RandomSearch()
        {
            Node search = Root;
            Random r = new Random();

            while (search.Level < 5)
            {
                List<Node> children = search.Children.Values.ToList();
                int index = r.Next(children.Count);

                if (children[index] != null && children[index].TotalSuccessors > 0)
                {
                    search = children[index];
                }
            }

            return search.Word;
        }

    }//End Class
}//End Namespace
