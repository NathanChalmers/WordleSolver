using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleSolver
{
    class Node
    {
        //Class Variables
        public Node Parent;
        public Dictionary<string, Node> Children;

        public string Value;
        public string Word = null;

        public int Level;
        public int TotalSuccessors;


        //Construction
        public Node(Node parent)
        {
            if (parent != null)
            {
                Parent = parent;
                Level = Parent.Level + 1;
            }
            else
            {
                Parent = null;
                Level = 0;
            }

            TotalSuccessors = 0;

            Children = new Dictionary<string, Node>();
        }

        //Add
        //Recursive function which adds new words to the tree, then updates the number of total sucessors
        public void Add(string word, string finalword = null)
        {
            //Update the Value of the Current Node
            Value = word[0].ToString();

            if (finalword == null)
            {
                finalword = word;
            }

            //Add the remaining letters in the word to the tree

            //Base Case: If there are no other letters to add return 1 as parents total successors
            if (word.Length == 1)
            {
                //If word length is 1, then we are at level 5 and there are no other valid words
                Word = finalword;
                TotalSuccessors = 1;
                return;
            }

            //Trim Word at Head and then add remainder recursively

            //Since we are not in the base case, we know that we have at least 1 more successor than us at this node
            TotalSuccessors++;
            
            word = word.Substring(1);
            string nextVal = word[0].ToString();

            if (!Children.ContainsKey(nextVal))
            {
                Children.Add(nextVal, new Node(this));
            }

            Children[nextVal].Add(word, finalword);
        }

        public void Remove(int successors = int.MinValue)
        {
            //Hard Delete Case: Actually Delete the Node Instead of Simply Update the Total Number of Scuccessors UpStream
            if (successors == int.MinValue)
            {
                this.Parent.Children[this.Value] = null;
                successors = this.TotalSuccessors;
            }

            this.TotalSuccessors = this.TotalSuccessors - successors;

            if (this.Parent != null)
            {
                this.Parent.Remove(successors);
            }
        }
    }
}
