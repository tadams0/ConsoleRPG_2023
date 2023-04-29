//using statements are used to tell the compiler which code we are using.
//using [namespace]

using ConsoleRPG_2023.RolePlayingGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

//Namespace is used to organize classes
//Format of making a namespace:
//namespace [name of the namespace]
namespace ConsoleRPG_2023
{
    //A class is used to organize methods and variables.
    //Format of making a class goes like this:
    //[Scope] class [Name of the class]
    class Program
    {
        //A method is used to perform some action or code.
        //Format of a method:
        //[Scope] [Static or omit it] [Return type, use void to say there is no return] [Name of the method](parameter list)
        private static void Main(string[] args)
        {

            //Begins the game, we use the static class RPGGame so all the RPG game logic is contained within it.
            RPGGame.Start();

        }


    }


}
