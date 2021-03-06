﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;

/*
 * The purpose of this program is to make either XML or Spider "saved games" using an arbitary or made up deck
 * program can have 0, 1 or 3 arguments
 * 0 args: write out possible commands (help)
 * 1 arg:  read arg as save file write out xml file.
 *    output is same filename with extension of .xml
 * rest are not coded yet but will be
 * if 3 args the first is input saved file and second is input XML we want to use
 * 1: game saved file for use as a templete
 * 2: xml file we want to put into the templete (basename must be different from 1)
 * 3: game filename that will have the xml file from (2) that was put into (1)
 * NOTE TO MYSELF, ANY OF THE MD HASHS (MD2,4 OR 5) CAN ENCODE A DECK OF 52 CARDS
 * 10/22/2020 WRITING OUT A CHECK TYPE VALUE TO SEE IF DUPLICATE FILES EXIST
 * TOP ROW OF CARDS TO BE CONVERTED TO 10 DIGIT HEX VALUE 0..E FOR ACE THROUGH KING WITH 0 FOR EMPTY COLUMN (PROBLEM IF EMPTY!)
 * */

namespace spider
{
    public class Program
    {
        static int XMLtoRead = -1;

        static void GiveHelp()
        {
            Console.WriteLine("makexml any.save - creates any.xml\r\n");
            Console.WriteLine("makexml any.save old.xml old.save - creates old.save\r\n");
        }

        static void Main(string[] args)
        {
            bool bIsThere = false;
            string stExt = ".SpiderSolitaireSave-ms";
            string stTemp, strSpiderBin0;
            string PathToDirectory;
            string stSrc="", stDef = "Spider Solitaire.SpiderSolitaireSave-ms";
            cSpinControl cSC;
            board InitialBoard;
            stTemp = System.Reflection.Assembly.GetEntryAssembly().Location; // path to executable
            if (args.Count() > 0)
            {
                stSrc = "\\" + args[0];
            }
            else //stSrc = "\\" + stDef;  - not searching for save file anymore
            {
                GiveHelp();
                Environment.Exit(0);
            }
            strSpiderBin0 = System.IO.Path.GetDirectoryName(stTemp) + stSrc;
            bIsThere = File.Exists(strSpiderBin0);
            if (bIsThere)
            {
                PathToDirectory = Path.GetDirectoryName(strSpiderBin0) + "\\";
                GlobalClass.strSpiderBin = strSpiderBin0;
                GlobalClass.strSpiderDir = PathToDirectory ;
                GlobalClass.strSpiderName = PathToDirectory + Path.GetFileNameWithoutExtension(strSpiderBin0);
                GlobalClass.strSpiderExt = Path.GetExtension(strSpiderBin0);
                cSC = new cSpinControl();   // this created the default xml "filename"
                InitialBoard = new board();
                cSC.Deck = new cBuildDeck(strSpiderBin0, XMLtoRead, ref cSC);
                if (args.Count() < 2) // just want to look at the xml
                {
                    cSC.Deck.GetBoardFromSpiderSave(ref InitialBoard);
                }
                else 
                {
                    string stDes = stDef;
                    if (args.Count() == 3) stDes = args[2];
                    if (!stDes.Contains(stExt)) stDes += stExt;
                    if (args[0] == stDes)
                    {
                        Console.WriteLine("cannot have same output file name as the input saved game\n");
                        Environment.Exit(0);
                    }
                    stTemp =  PathToDirectory + Path.GetFileNameWithoutExtension(args[1]);
                    if (GlobalClass.strSpiderName == stTemp)
                    {
                        Console.WriteLine("should not have same input saved game name as the input xml filename\n");
                        Console.WriteLine("or input xml file will be over written as that filename used for temp storage\n");
                        Environment.Exit(0);
                    }
                    GlobalClass.strSpiderOutputBinary  = cSC.BIN_Diag_filename = PathToDirectory + "\\" + stDes;
                    cSC.Deck.GetBoardFromSpiderSave(ref InitialBoard);  // read in the saved game and also create the matching xml
                    cSC.XML_Diag_filename = PathToDirectory + args[1];                   
                    cSC.Deck.WriteBoardMergingXML();
                }

            }
        }
    }
}
