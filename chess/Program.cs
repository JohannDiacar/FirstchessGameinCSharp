// See https://aka.ms/new-console-template for more information
using System;
using System.Runtime.CompilerServices;

public class Chess
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Initialisation!");
        Board board = new Board();
        board.Init();
        board.Init();
        board.Reset();
        board.Init();
    }
}