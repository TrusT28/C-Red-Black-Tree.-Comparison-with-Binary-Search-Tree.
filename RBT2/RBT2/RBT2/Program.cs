//Author Rustam Fadeev @TrusT_28

//This program is my semester project for the first year.
//It contains full implementation of the Reb-Black Tree and Binary Search tree.
//Purpose of the project is to show how much the Reb-Black Tree more efficient than Binary Search Tree
//When it comes to adding ascending list of numbers.
//Also it tests trees for deleting elements and adding random numbers.
//
//After the tests, it is possible to work with the Red Black tree itself: adding, searching and deleting elements.

//The main difference between the Red-Black Tree (RBT) and the Binary Search Tree (BST) is that the Red-Black tree is always balanced.
//RBT changes its construction after each added value, so it stays a tree (Left Child is smaller, Right child is bigger) and it is balanced.
//Balanced means the structure of the tree assures good complexity time for each operation.
//The biggest problem of BST is that, if you add a list of ascending numbers, it will degenerate to the linked list,
//reducing complexity time to O(N) for each operation. RBT, on the other hand, has complexity O(log(N)) always for each operation.

//Implementations are generic (supports any data type) and recursive.

//Trees are made this way:
//              HEAD
//  LEFT(Less)     RIGHT(Bigger)

using static System.Console;
using System;
using System.Linq;
namespace TreeComparison
{
    class RedNode<T> //Node for Red-Black Tree
    {
        public T value; //Node's number
        public bool Red = false; //Red - True, Black - False
        public RedNode<T> parent; //Parent node
        public RedNode<T> left; //Left Child
        public RedNode<T> right; //Right Child
    }
    class Node<T> //Node for Binary Search Tree
    {
        public T value; //Node's number
        public Node<T> left; //Left Child
        public Node<T> right; //Right Child
    }
    interface Set<T> //Generic interface that each tree must implement
    {
        bool add(T val); //Add "val" from the tree. False if element is already in the list
        bool remove(T val); //Remove "val" from the tree. False if element wasn't in the list
        bool contains(T val); //Check if "val" is in the tree
        void print(); //Print the tree
        bool CheckTree(); //Check if tree is correct
    }
    class Tree<T> : Set<T> //Generic implementation of Binary Search Tree
        where T : IComparable<T>
    {
        Node<T> root; //Root of the tree

        public bool CheckTree() => true; //If CheckTree if correct - return True

        public bool add(T val) => add(val, ref root); 
        bool add(T val, ref Node<T> head) //Add the value "val" to the tree that starts with Node "head"
        {
            if (head == null) //Tree is empty. Create the root.
            {
                head = new Node<T>();
                head.value = val;
                head.left = null;
                head.right = null;
                return true;
            }
            else if (head.value.CompareTo(val) == 0) return false; //value "val" is already in the tree.
            else if (head.value.CompareTo(val) < 0) return (add(val, ref head.right)); //"val" is bigger than than current Node. Go right.
            else return (add(val, ref head.left)); //"val" is smaller than current Node. Go left.
        }

        public bool remove(T val) => remove(ref root, val); //Remove "val" from Tree

        bool remove(ref Node<T> n, T val) //Remove "val" from tree
        {
            if (n == null) return false;  // not found

            if (val.CompareTo(n.value) < 0) return remove(ref n.left, val); //"val" is less. Go left.
            if (val.CompareTo(n.value) > 0) return remove(ref n.right, val); //"val" is bigger. Go right

            if (n.left == null)
                n = n.right;
            else if (n.right == null)
                n = n.left;
            else n.value = removeSmallest(ref n.right);

            return true;
        }

        T removeSmallest(ref Node<T> head) //Remove smallest value from the tree. Return the value
        {
            if (head.left == null) //Current value is the smallest
            {
                T small = head.value;
                if (head.right != null) //Right Node is non-empty. Change the tree accordingly
                    head = head.right;
                else head = null; //Right Node is empty. Just delete
                return (small);
            }
            else return (removeSmallest(ref head.left)); //Current value is not the smallest. Go left
        }

        public bool contains(T val) =>
            contains(val, root);
        bool contains(T val, Node<T> head) //Check if "val" is in the tree
        {
            if (head == null) return false;
            else if (head.value.CompareTo(val) == 0) return true;
            else if (head.value.CompareTo(val) < 0) return (contains(val, head.right));
            else return (contains(val, head.left));
        }

        static void print(Node<T> n, int depth) //print the tree
        {
            if (n == null) return;
            print(n.right, depth + 1);
            WriteLine(new string(' ', depth * 2) + n.value);
            print(n.left, depth + 1);
        }

        public void print() => print(root, 0);
    }

    class RedBlackTree<T> : Set<T> //Generic implementation of the Red-Black Tree
        where T : IComparable<T>
    {
        RedNode<T> nil; //Special Node of Red-Black Tree. Every leaf has it.
        RedNode<T> root; //Root of the tree
        public RedBlackTree() //Constructor of the Red-Black tree. Set default values.
        {
            root = new RedNode<T>();
            nil = new RedNode<T>();
            nil.left = nil;
            nil.right = nil;
            nil.parent = nil;
            root = nil;
        }

        bool isRed(RedNode<T> head) => head != nil && head.Red; //Check if the Node is Red

        void Left_Rotation(RedNode<T> head) //Special operation of Red-Black Tree. Left-Rotate the node "Head"
        { 
            RedNode<T> Right = head.right; 
            head.right = Right.left; 

            if (Right.left != nil) Right.left.parent = head; 

            Right.parent = head.parent;

            if (head.parent == nil)
                root = Right;
            else if (head.parent.left == head)
                head.parent.left = Right;
            else head.parent.right = Right;

            Right.left = head;
            head.parent = Right;
        }
        void Right_Rotation(RedNode<T> head) //Special operation of Red-Black Tree. Right-Rotate the node "Head"
        {
            RedNode<T> Left = head.left;
            head.left = Left.right;
            if (Left.right != nil) Left.right.parent = head;
            Left.parent = head.parent;
            if (head.parent == nil)
                root = Left;
            else if (head.parent.right == head)
                head.parent.right = Left;
            else head.parent.left = Left;
            Left.right = head;
            head.parent = Left;
        }

        void Insert_Balance(RedNode<T> head) //Self balance of the Red-Black Tree, i.e change the tree, so it is always balanced.
        {
            while (head.parent.Red)
            {
                if (head.parent == head.parent.parent.left)
                {
                    RedNode<T> uncle = head.parent.parent.right;
                    if (!isRed(uncle))
                    {
                        if (head == head.parent.right)
                        {
                            head = head.parent;
                            Left_Rotation(head);

                        }
                        head.parent.Red = false;
                        head.parent.parent.Red = true;
                        Right_Rotation(head.parent.parent);
                        if (root == head || head.parent.parent == nil)
                            break;
                    }
                    else
                    {
                        head.parent.Red = false;
                        uncle.Red = false;
                        head.parent.parent.Red = true;
                        head = head.parent.parent;
                        if (root == head || head.parent.parent == nil)
                            break;
                    }
                }
                else
                {
                    RedNode<T> uncle = head.parent.parent.left;
                    if (!isRed(uncle))
                    {
                        if (head == head.parent.left)
                        {
                            head = head.parent;
                            Right_Rotation(head);
                        }
                        head.parent.Red = false;
                        head.parent.parent.Red = true;
                        Left_Rotation(head.parent.parent);
                        if (root == head || head.parent.parent == nil)
                            break;
                    }
                    else
                    {
                        head.parent.Red = false;
                        uncle.Red = false;
                        head.parent.parent.Red = true;
                        head = head.parent.parent;
                        if (root == head || head.parent.parent == nil)
                            break;
                    }
                }
            }
            root.Red = false;
        }
        public bool add(T val) => add(val, ref root, nil); //Add "val" to the tree, that starts with "root"
        bool add(T val, ref RedNode<T> head, RedNode<T> parent)
        {
            if (head == nil) //Free spot to add the "val"
            {
                head = new RedNode<T>(); //Create the node
                head.Red = true; //Mark the recently-added Node Red
                head.value = val;
                head.left = nil;
                head.right = nil;
                head.parent = parent;
                if (parent == nil) //Current node is root of the tree
                    root = head;
                Insert_Balance(head); //Balance the tree, so it doesn't become a linked list.
                return true;
            }
            else if (head.value.CompareTo(val) == 0) return false;
            else if (head.value.CompareTo(val) < 0) return (add(val, ref head.right, head));
            else return (add(val, ref head.left, head));
        }

        void Remove_Balance(RedNode<T> head) //Self-Balance after removing some value
        {
            while (head != root && !isRed(head))
            {
                if (head == head.parent.left)
                {
                    RedNode<T> sibling = head.parent.right;
                    if (isRed(sibling))
                    {
                        sibling.Red = false;
                        head.parent.Red = true;
                        Left_Rotation(head.parent);
                        sibling = head.parent.right;
                    }
                    if (!isRed(sibling.left) && !isRed(sibling.right))
                    {
                        sibling.Red = true;
                        head = head.parent;
                    }
                    else
                    {
                        if (!isRed(sibling.right))
                        {
                            sibling.left.Red = false;
                            sibling.Red = true;
                            Right_Rotation(sibling);
                            sibling = head.parent.right;
                        }
                        sibling.Red = head.parent.Red;
                        head.parent.Red = false;
                        sibling.right.Red = false;
                        Left_Rotation(head.parent);
                        head = root;
                    }

                }
                else
                {
                    RedNode<T> sibling = head.parent.left;
                    if (isRed(sibling))
                    {
                        sibling.Red = false;
                        head.parent.Red = true;
                        Right_Rotation(head.parent);
                        sibling = head.parent.left;
                    }
                    if (!isRed(sibling.left) && !isRed(sibling.right))
                    {
                        sibling.Red = true;
                        head = head.parent;
                    }
                    else
                    {
                        if (!isRed(sibling.left))
                        {
                            sibling.right.Red = false;
                            sibling.Red = true;
                            Left_Rotation(sibling);
                            sibling = head.parent.left;
                        }
                        sibling.Red = head.parent.Red;
                        head.parent.Red = false;
                        sibling.left.Red = false;
                        Right_Rotation(head.parent);
                        head = root;
                    }
                }
            }
            head.Red = false;
        }
        public bool remove(T val) => remove(ref root, val);
        bool remove(ref RedNode<T> n, T val)
        {
            if (n == nil) return false;  //"val" not found

            if (val.CompareTo(n.value) < 0) return remove(ref n.left, val);
            if (val.CompareTo(n.value) > 0) return remove(ref n.right, val);
            if (val.CompareTo(n.value) == 0) //found the value "val"
            {
                bool wasRed = isRed(n); 
                RedNode<T> x = null;
                if (n.left == nil)
                {
                    n.right.parent = n.parent;
                    n = n.right;
                }
                else if (n.right == nil)
                {
                    n.left.parent = n.parent;
                    n = n.left;
                }
                else n.value = removeSmallest(ref n.right, out wasRed, out x);
                if (!wasRed) //Self-Balance
                    if (x == null) Remove_Balance(n);
                    else Remove_Balance(x);
            }
            return true;
        }

        T removeSmallest(ref RedNode<T> head, out bool red, out RedNode<T> x) //Remove the smallest value from the tree. Return it ("x")
        {
            if (head.left == nil)
            {
                T small = head.value;
                red = isRed(head);
                x = head.right;
                head.right.parent = head.parent;
                if (head.right != nil)
                    head = head.right;
                else head = nil;
                return (small);
            }
            else return (removeSmallest(ref head.left, out red, out x));
        }
        public bool contains(T val) //True if "val" is in the tree
        {
            return (contains(val, root));
        }
        bool contains(T val, RedNode<T> head)
        {
            if (head == nil) return false; //not in the tree
            else if (head.value.CompareTo(val) == 0) return true; //in the tree
            else if (head.value.CompareTo(val) < 0) return (contains(val, head.right));
            else return (contains(val, head.left));
        }

        public void print() //Print the tree
        {
            print(root, 0);
            ForegroundColor = ConsoleColor.White;
        }
        void print(RedNode<T> head, int spaces) 
        {
            if (head == nil)
                return;
            print(head.right, spaces + 1);
            for (int i = 0; i <= spaces; i++)
                Write(" ");
            if (head.Red) ForegroundColor = ConsoleColor.Red;
            else ForegroundColor = ConsoleColor.White;
            WriteLine($"{head.value}");
            print(head.left, spaces + 1);
        }
        public bool CheckTree() //Check if tree is correct. Takes time.
        {
            if (root == nil) return true;
            return (ColorCheck(root) && computeBlackHeight(root) != -1);
        }
        bool ColorCheck(RedNode<T> head) //Check if coloring of the Red-black tree is correct.
        {
            if (head == nil) return true;
            if (isRed(head) && (isRed(head.left) || isRed(head.right))) return false;
            return (ColorCheck(head.left) && ColorCheck(head.right));
        }
        int computeBlackHeight(RedNode<T> Head) //Check if coloring of the Red-Black tree is correct
        {
            if (Head == nil)
                return 0;
            int leftHeight = computeBlackHeight(Head.left);
            int rightHeight = computeBlackHeight(Head.right);
            int add;
            if (Head.Red) add = 0;
            else add = 1;
            if (leftHeight == -1 || rightHeight == -1 || leftHeight != rightHeight)
                return -1;
            else
                return leftHeight + add;
        }
    }



    public class Testing //Comparing RBT and BST
    {
        int numberOfElements = 0; //Number of ascending elements each tree can have. Set by user
        int numberOfRandomElements = 0; //Number of Random elements each tree can have. Set by user
        void Checking(RedBlackTree<int> tree) //Purely for debugging. Takes a lot of time to check.
        {
            if (!tree.CheckTree())
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("ERROR: ");
                ForegroundColor = ConsoleColor.White;
                WriteLine("Red -Black Tree is incorrect! Exiting the program");
                Environment.Exit(1);
            }
        }

        void Add(Set<int> T, int[] rand, out TimeSpan diff) //Add value to the tree "T". array "rand" contains random numbers.
        {
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();
            //Text
            string B = "Binary Search Tree";
            string R = "Red-Black Tree";
            ForegroundColor = ConsoleColor.Cyan;
            Write("Adding ");
            ForegroundColor = ConsoleColor.White;
            if (rand == null)
                Write(numberOfElements + " ascending numbers into ");
            else
                Write(numberOfRandomElements + " random numbers into ");
            if (T is Tree<int>) WriteLine(B);
            else WriteLine(R);

            //Add values
            StartTime = DateTime.Now;//Start counting
            if (rand == null) //Add ascending numbers
                for (int i = 0; i <= numberOfElements; i++)
                        T.add(i);
            else //Add random numbers
                for (int i = 0; i < rand.Length; i++)
                    T.add(rand[i]);
            EndTime = DateTime.Now;//Stop counting

            //Compare values
            diff = EndTime - StartTime;
            WriteLine($"It took {diff.TotalMilliseconds} milliseconds");
            WriteLine();
        }

        void Comparison(TimeSpan BSTTime, TimeSpan RBTTime) //Compare 2 finish times
        {
            ForegroundColor = ConsoleColor.Red;
            if (Math.Round((BSTTime.TotalMilliseconds / RBTTime.TotalMilliseconds), 5) == 1) 
                WriteLine("There was no difference");
            else if (BSTTime > RBTTime) 
                WriteLine($"Red-Black Tree was {Math.Round((BSTTime.TotalMilliseconds / RBTTime.TotalMilliseconds), 2)} times faster ");
            else 
                WriteLine($"Binary Search Tree was {Math.Round((RBTTime.TotalMilliseconds / BSTTime.TotalMilliseconds), 2)} times faster");
            ForegroundColor = ConsoleColor.White;
            WriteLine();
        }

        void Delete(Set<int> T, int[] rand, out TimeSpan diff) //Delete elements from tree "T"
        {
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();

            //Text
            string B = "Binary Search Tree";
            string R = "Red-Black Tree";
            ForegroundColor = ConsoleColor.DarkYellow;
            Write("Removing ");
            ForegroundColor = ConsoleColor.White;
            if (rand == null)
                Write(numberOfElements + " ascending numbers from ");
            else
                Write(numberOfRandomElements + " random numbers from ");
            if (T is Tree<int>) WriteLine(B);
            else WriteLine(R);
            
            //Delete
            StartTime = DateTime.Now;
            if (rand == null)
                for (int i = 0; i <= numberOfElements; i++)
                    T.remove(i);
            else
                for (int i = 0; i < rand.Length; i++)
                    T.remove(rand[i]);
            EndTime = DateTime.Now;

            //Count Time
            diff = EndTime - StartTime;
            WriteLine($"It took {diff.TotalMilliseconds} milliseconds");
            WriteLine();
        }

        public void Start() //Start Comparison Testing
        {
            Set<int> RedBlackTree = new RedBlackTree<int>(); //Instance of RedBlackTree
            Set<int> BinarySearchTree = new Tree<int>(); //Instance of Binary Search Tree

            TimeSpan diffBST = new TimeSpan(); //For Binary Search Tree
            TimeSpan diffRBT = new TimeSpan(); //For Red-Black Tree

            //How many elements do you want to add to trees?
            WriteLine("How many *ascending* numbers do you want to add into trees?");
            WriteLine("Warning! Too many can cause StackOverflow in Binary Search Tree! Recommended number: 5000");
            string s;
            s = ReadLine();
            while(true)
            {
                if (s.All(char.IsDigit))
                {
                    numberOfElements = int.Parse(s);
                    break;
                }
                else
                {
                    WriteLine("Error: not a number. Please, try again.");
                    s = ReadLine();
                }
            }

            WriteLine("How many *random* numbers do you want to add into trees?");
            WriteLine("Warning! Too many can cause StackOverflow in Binary Search Tree! Recommended number: 5 million");
            s = ReadLine();
            while (true)
            {
                if (s.All(char.IsDigit))
                {
                    numberOfRandomElements = int.Parse(s);
                    break;
                }
                else
                {
                    WriteLine("Error: not a number. Please, try again.");
                    s = ReadLine();
                }
            }

            //Create array of random numbers
            Random r = new Random();
            int[] rand = new int[numberOfRandomElements];
            for (int i = 0; i < rand.Length; i++)
                rand[i] = r.Next(numberOfRandomElements+1);
            
            //Testing
            Add(BinarySearchTree, null, out diffBST);
            Add(RedBlackTree, null, out diffRBT);
            Comparison(diffBST, diffRBT);

            Delete(BinarySearchTree, null, out diffBST);
            Delete(RedBlackTree, null, out diffRBT);
            Comparison(diffBST, diffRBT);

            Add(BinarySearchTree, rand, out diffBST);
            Add(RedBlackTree, rand, out diffRBT);
            Comparison(diffBST, diffRBT);

            Delete(BinarySearchTree, rand, out diffBST);
            Delete(RedBlackTree, rand, out diffRBT);
            Comparison(diffBST, diffRBT);


            //Working with Red Black Tree yourslef
            WriteLine("Would you like to continue working with Red-Black Tree? (Y/N)");
            while (true)
            {
                s = ReadLine();
                if (s == "n" || s == "N") return;
                if (s == "y" || s == "Y") break;
                WriteLine("Incorrect command");
            }
            bool Result = false;
            WriteLine("For info write 'Help'");
            while (true)
            {
                s = ReadLine();
                if (s == null) break;

                string[] command = s.Split(' ');
                switch (command[0])
                {
                    case "Help":
                        WriteLine("To insert element write I [integer]");
                        WriteLine("To delete element write D [integer]");
                        WriteLine("To know whether the element in the tree or not write Q [integer]");
                        WriteLine("To print the tree write P");
                        WriteLine("To check correctness of the tree write C");
                        WriteLine("To exit write Exit");
                        break;
                    case "Exit":
                        WriteLine("Exiting...");
                        Environment.Exit(0);
                        break;
                    case "I":
                        WriteLine(s);
                        Result = RedBlackTree.add(int.Parse(command[1]));
                        if (Result) WriteLine("Element inserted");
                        else WriteLine("Element was already in the tree");
                        break;
                    case "D":
                        WriteLine(s);
                        Result = RedBlackTree.remove(int.Parse(command[1]));
                        if (Result) WriteLine("Element deleted");
                        else WriteLine("Element was not in the tree");
                        break;
                    case "Q":
                        WriteLine(s);
                        Result = RedBlackTree.contains(int.Parse(command[1]));
                        if (Result) WriteLine("Present");
                        else WriteLine("Absent");
                        break;
                    case "P":
                        WriteLine(s);
                        RedBlackTree.print();
                        break;
                    case "C":
                        Write("Checking correctness of Red-Black Tree: ");
                        WriteLine(RedBlackTree.CheckTree());
                        break;
                }
            }
        }
    }
    class Program
    {
        static void Main()
        {
            Testing t = new Testing();
            t.Start();
            WriteLine("Would you like to perform testing again?");
            String s;
            while (true)
            {
                s = ReadLine();
                if (s == "n" || s == "N") return;
                if (s == "y" || s == "Y") t.Start();
                WriteLine("Incorrect command");
            }
        }
    }
}

