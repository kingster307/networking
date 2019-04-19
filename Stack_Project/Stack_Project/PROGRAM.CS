using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stack_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            Boolean exit = false;
            int addtostack;
            int numinstack;
           // Stack<int> mystack = new Stack<int>();
            int timecount =0;
            string command = null ;
            int commandnum = 0;
            Boolean error = false;
             int arraycount=0;
             List<int> mystack = new List<int>();
             string typeofstack;
             int switchstack=-1;
             int errorwatch = 0;
             Boolean reversedyet = false;

            //make them switch between the various options use switches

             while (error == true || errorwatch == 0)
             {
                 if (error == true)
                 {
                     Console.WriteLine("Error Please enter a valid value\n");
                     
                 }
                 Console.WriteLine("What type of stack do you want? \n      Fifo, Lifo, or circ?\n");
                 typeofstack = (Console.ReadLine());
                 error = false;
                 switch (typeofstack)
                 {
                     case "FIFO":
                     case "fifo":
                     case "Fifo":
                     case "fiFo":

                         switchstack = 0;
                         break;

                     case "LIFO":
                     case "Lifo":
                     case "liFo":
                     case "lifo":

                         switchstack = 1;
                         break;

                     case "circ":
                     case "Circ":

                         switchstack = 2;
                         break;
                 }
                 if (switchstack == -1)
                 {
                     error = true;
                     typeofstack = null;
                 }
                 errorwatch += 1;
             }
            if (timecount == 0)
                {
                    Console.WriteLine("\nWhat would you like to do?");
                    Console.WriteLine("Commands:  See Stack   Add to Stack  Count Stack  Show Commands  Pop  Exit\n___________________________________________________\n");
                    command = Console.ReadLine();
                }
            
            while (exit == false)
            
            {
               // int[] mystack = new int[arraycount +1];     
                if (timecount > 0)
                {
                    if (error == false && exit == false)
                    {
                        Console.WriteLine("\nWhat would you like to do?\n_______________________________\n");
                    }
                    command = Console.ReadLine();
                }
                error = false;
                
                switch (command)
                {
                    case "See Stack":
                    case "see stack":
                        commandnum = 1;
                        break;

                    case "Add to Stack":
                    case "add to stack":
                        commandnum = 2;
                        break;

                    case "Count Stack":
                    case "count stack":
                        commandnum = 3;
                        break;

                    case "Exit":
                    case "exit":
                        commandnum = 4;
                        break;

                    case "show commands":
                    case "Show Commands":
                    case "Commands":
                    case "commands":
                    case "command":
                    case "Command":
                        commandnum = 5;
                        break;
                
                    case "POP":
                    case "Pop":
                    case"pop":
                        commandnum =6;
                            break ;
                
                }



                switch (commandnum)
                {
                    default:
                        Console.WriteLine("\nError please enter a valid command\n");
                       error =true ; 
                        break;

                    //make stack visible
                        //error here everytime that its pressed it reverses
                    case 1:
                      
                    
                    //fifo
                        if (mystack.Count  > 0 )
                        {
                           
                            
                                //fifo
                                if (switchstack== 0)
                                {
                                    foreach (int stacks in mystack )
                                         {
                                             Console.WriteLine(stacks);
                                         }
                                }
                                           //lifo
                                         else if (switchstack ==1)
                                {

                                    if (reversedyet == false)
                                    {
                                        mystack.Reverse();
                                    }
                                             reversedyet = true;
                                    foreach (int stacks in mystack )
                                         {
                                             Console.WriteLine(stacks);
                                         }
                                     
                                         }
                               //circ
                               else if (switchstack ==2)
                                {
                               //find out how to do the circle 
                               }

                                //else
                                //{
                                 //  Console .WriteLine ("You've messed up so you get fifo");
                                  // switchstack =0;
                                   //Console .Read ();
                               //}

                        }
                        else
                        {
                            Console.WriteLine("\nStack is empty");
                           
                        }
                        break;
                    //add something to stack
                    case 2:
                        Console.WriteLine("\nWrite an integer to add to stack");
                      bool  testint = int.TryParse(Console.ReadLine (),out addtostack) ;
                      if (testint == false)
                      {
                          Console.WriteLine("Error Please enter a valid integer");
                          Console.Read();
                      }
                      else 
                      {
                          mystack.Add (addtostack) ;
                          arraycount += 1;
                      }
                        break;

                    //count number of things in stack
                    case 3:
                        if (mystack.Count > 0)
                        {
                          // foreach (
                            numinstack = mystack.Count;
                            if (mystack.Count   > 1)
                            {
                                Console.WriteLine("There are " + numinstack + " Elements in the stack");
                            }
                            else
                            {
                                Console.WriteLine("There is " + numinstack + " Elements in the stack");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nStack is empty");
                            
                        }
                        break;

                    //exit
                    case 4:
                        exit = true;
                        break;
                        //see command
                    case 5:
                        Console.WriteLine("Commands:  See Stack   Add to Stack  Count Stack  Show Commands  Exit");
                             break ;
                
                    //pop
                    case 6:

                        //fifo 0
                        //take it off index 0 
                             if (switchstack == 0 || switchstack ==1)
                             {
                                 mystack.RemoveAt (0);
                             }


                        //lifo 1
                        //take it off length---to index 
                           //  if (switchstack == 1)
                            // {
                             
                                
                            // }
                            
                            
                       //circ 2
                        //find out how circle works 


                             if (switchstack == 2)
                             { 
                             
                             }
                      //  {}

                             break;
                
                }


                timecount += 1;
                command = null;
                commandnum = 0;
            }
        }
    }
}
//need to do pop and circ for see stack

