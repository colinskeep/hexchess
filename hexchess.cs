﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HexC
{
    public enum PiecesEnum { Pawn, Knight, Castle, Queen, King }
    public enum ColorsEnum { White, Tan, Black }

    // these numbers are the axial coordinate system on a flat-topped hex board.
    // let's change them to pointy-topped!
    // http://www.redblobgames.com/grids/hexagons/

    class KingStatic : PieceStatic
    {
        public static BoardLocationList CouldGoIfOmnipotent(BoardLocation fromHere)
        {
            int[,] JumpOptions = {
                { 0, -1 }, { 1, -1 }, {1, 0 }, {0,1 }, {-1, 1 }, {-1,0 }
            };
            return CookUpLocations(fromHere, JumpOptions);
        }

    }

    class KnightStatic : PieceStatic
    {
        public static BoardLocationList CouldGoIfOmnipotent(BoardLocation fromHere)
        {
            int[,] JumpOptions = {
                                    { 1, -3 }, { 2, -3 }, { -2, -1 }, { -1, -2 }, { 3, -2 }, { 3, -1 },
                                    { 2, 1 }, { 1, 2 }, { -2, 3 }, {-1, 3 }, {-3, 1 }, {-3, 2 }
            };
            return CookUpLocations(fromHere, JumpOptions);
        }
    }

    class CastleStatic : PieceStatic
    {
        public static List<BoardLocationList> ListOfSequencesOfSpots(BoardLocation loc)
        {
            List<BoardLocationList> ll = new List<BoardLocationList>();

            // SEQUENCE of these matters, radiating out from the piece.
            int[,] StarOne = { { 1, -1 }, { 2, -2 }, { 3, -3 }, { 4, -4 }, { 5, -5 }, { 6, -6 }, { 7, -7 }, { 8, -8 }, { 9, -9 }, { 10, -10 } };
            int[,] StarTwo = { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 } };
            int[,] StarThr =  { { 0, 1 }, { 0, 2 }, { 0, 3 }, { 0, 4 }, { 0, 5 }, { 0, 6 }, { 0, 7 }, { 0, 8 }, { 0, 9 }, { 0, 10 } };
            int[,] StarFou = { { -1, 1 }, { -2, 2 }, { -3, 3 }, { -4, 4 }, { -5, 5 }, { -6, 6 }, { -7, 7 }, { -8, 8 }, { -9, 9 }, { -10, 10 } };
            int[,] StarFiv = { { -1, 0 }, { -2, 0 }, { -3, 0 }, { -4, 0 }, { -5, 0 }, { -6, 0 }, { -7, 0 }, { -8, 0 }, { -9, 0 }, { -10, 0 } };
            int[,] StarSix =  { { 0, -1 }, { 0, -2 }, { 0, -3 }, { 0, -4 }, { 0, -5 }, { 0, -6 }, { 0, -7 }, { 0, -8 }, { 0, -9 }, { 0, -10 } };

            ll.Add(CookUpLocations(loc, StarOne));
            ll.Add(CookUpLocations(loc, StarTwo));
            ll.Add(CookUpLocations(loc, StarThr));
            ll.Add(CookUpLocations(loc, StarFou));
            ll.Add(CookUpLocations(loc, StarFiv));
            ll.Add(CookUpLocations(loc, StarSix));

            return ll;
        }
    }

    class PieceStatic
    {
        static protected BoardLocationList CookUpLocations(BoardLocation fromHere, int[,] jumpOpts)
        {
            BoardLocationList spots = new BoardLocationList();

            for (int iSet = 0; iSet < jumpOpts.GetLength(0); iSet++)
            {
                BoardLocation b = new BoardLocation(fromHere, new BoardLocation(jumpOpts[iSet, 0], jumpOpts[iSet, 1]));
                spots.Add(b);
            }
            return spots;
        }
    }

    public class BoardLocation
    {
        public BoardLocation(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public BoardLocation(BoardLocation b1, BoardLocation b2)
        {
            this.q = b1.q + b2.q;
            this.r = b1.r + b2.r;
        }

        public bool IsValidLocation()
        {
            if (q > 5) return false;
            if (r > 5) return false;
            if (q < -5) return false;
            if (r < -5) return false;
            if (q + r > 5) return false;
            if (q + r < -5) return false;

            return true;
        }

        public int Q { get { return q; } }
        public int R { get { return r; } }

        int q;
        int r;
    }

    class BoardLocationList : List<BoardLocation>
    {
        public bool ContainsTheLocation(BoardLocation bToMatch)
        {
            foreach (BoardLocation bl in this)
            {
                if (bl.Q == bToMatch.Q)
                    if (bl.R == bToMatch.R)
                        return true;
            }
            return false;
        }
    }

    public class Piece
    {
        public Piece(PiecesEnum pt, ColorsEnum c)
        {
            this.pieceType = pt;
            this.pieceColor = c;
        }

        protected PiecesEnum pieceType;
        protected ColorsEnum pieceColor;

        public PiecesEnum PieceType { get { return this.pieceType; } }
        public ColorsEnum Color { get { return this.pieceColor; } }
    }

    public class PlacedPiece : Piece
    {
        public PlacedPiece(PiecesEnum pt, ColorsEnum c, int q, int r) : base(pt, c)
        {
            this.q = q;
            this.r = r;
        }

        public PlacedPiece(PlacedPiece p, BoardLocation bl) : base(p.PieceType, p.pieceColor) // clone me but to a new spot
        {
            this.q = bl.Q;
            this.r = bl.R;
        }

        public BoardLocation Location { get { return new BoardLocation(q, r); } }

        public bool DeepEquals(PlacedPiece p)
        {
            if (this.pieceColor != p.pieceColor) return false;
            if (this.pieceType != p.pieceType) return false;
            if (this.q != p.q) return false;
            if (this.r != p.r) return false;
            return true;
        }

        // my happy variables
        int q;
        int r;
    }

    enum EventTypeEnum { Add, Remove };

    class PieceEvent
    {
        public PieceEvent(PlacedPiece p, EventTypeEnum t)
        { this.p = p; this.t = t; }

        PlacedPiece p;
        EventTypeEnum t;

        public PlacedPiece Regarding { get { return p; } }
        public EventTypeEnum Type { get { return t; } }
    }
    

    class Board
    {
        // MEMBERS
        List<PlacedPiece> placedPieces = new List<PlacedPiece>();
        List<Piece> sidelined = new List<Piece>();

        // PROPERTIES
        public List<PlacedPiece> PlacedPieces { get { return placedPieces; } }

        // CONSTRUCTORS
        public Board() { }
        public Board(Board cloneMe)
        {
            foreach (PlacedPiece p in cloneMe.placedPieces)
            {
                placedPieces.Add(p);
            }
            foreach (Piece p in cloneMe.sidelined)
            {
                sidelined.Add(p);
            }
        }

        // METHODS
        public void Add(PlacedPiece p)
        {
            // i'm gonna prevent you from adding an impossible number of pieces,
            // just cuz i roll that way.
            switch (p.PieceType)
            {
                case PiecesEnum.King:
                    // if this is the king we're adding, well, let's make sure there isn't one already.
                    Debug.Assert(null == this.FindPiece(PiecesEnum.King, p.Color));
                    break;

                    // others need checks
            }
            placedPieces.Add(p);
        }

        public void Remove(PlacedPiece p)
        {
            foreach (var placed in placedPieces)
            {
                if (placed.DeepEquals(p))
                {
                    placedPieces.Remove(placed);
                    return;
                }
            }
            Debug.Assert(false); // hey why remove what isn't there?
        }

        public List<PlacedPiece> PlacedPiecesThisColor(ColorsEnum col)
        {
            List<PlacedPiece> myCol = new List<PlacedPiece>();

            foreach (PlacedPiece p in placedPieces)
            {
                if (p.Color == col)
                    myCol.Add(p);
            }
            return myCol;
        }


        PlacedPiece AnyoneThere(BoardLocation b)
        {
            foreach (PlacedPiece pp in placedPieces)
            {
                if (pp.Location.Q == b.Q)
                    if (pp.Location.R == b.R)
                        return pp;
            }
            return null;
        }

        BoardLocationList YankSpotsThatArentBoardSpots(BoardLocationList options)
        {
            BoardLocationList realOptions = new BoardLocationList();

            foreach (BoardLocation b in options)
            {
                if (b.IsValidLocation())
                    realOptions.Add(b);
            }

            return realOptions;
        }

        BoardLocationList YankSpotsOfThisColor(BoardLocationList options, ColorsEnum c)
        {
            BoardLocationList realOptions = new BoardLocationList();

            foreach (BoardLocation b in options)
            {
                PlacedPiece p = AnyoneThere(b);
                if (null != p)
                    if (p.Color == c)
                        continue;

                realOptions.Add(b);
            }
            return realOptions;
        }

        PlacedPiece FindPiece(PiecesEnum type, ColorsEnum c)
        {
            Debug.Assert(type == PiecesEnum.King || type == PiecesEnum.Queen);

            foreach (PlacedPiece p in this.placedPieces)
            {
                if (p.PieceType == type)
                    if (p.Color == c)
                        return p;
            }
            return null;
        }

        BoardLocationList YankSpotsThatPutMeInCheck(BoardLocationList options, PlacedPiece p)
        {
//            Console.WriteLine("A {1} {0} removes spots that put their team in check.", p.PieceType.ToString(), p.Color.ToString());

            BoardLocationList realOptions = new BoardLocationList();

            foreach (BoardLocation bl in options)
            {
                // Make a hypothetical board
                Board bHypothetical = new Board(this);     // clone meeeee
                bHypothetical.Remove(p);                     // take me off.
                bHypothetical.Add(new PlacedPiece(p, bl));   // put me on at the destination

                // I wanna see this board.
                Program.ShowBoard(bHypothetical);

                if (false == bHypothetical.InCheck(p.Color)) // see if i'm in check
                    realOptions.Add(bl);
            }
            return realOptions;
        }

        bool CanAttack(PlacedPiece p, PlacedPiece pVictim)
        {
            if (p.Color == pVictim.Color)
                return false;// can't attack my kind.

//            Console.WriteLine("A {3} {0} at {1},{2} wonders where it can reach.", p.PieceType, p.Location.Q, p.Location.R, p.Color);
            BoardLocationList options = WhereCanIReach(p);
            if (options.ContainsTheLocation(pVictim.Location))
                return true;

            return false;
        }

        // Does the placed piece threaten the specific king NOW? How can we tell if we don't know that it puts us in check?
        bool CanAttackNowNoRecurse(PlacedPiece p, PlacedPiece pVictim)
        {
            if (p.Color == pVictim.Color)
                return false;// can't attack my kind.

//            Console.WriteLine("A {3} {0} at {1},{2} wonders where it can reach.", p.PieceType, p.Location.Q, p.Location.R, p.Color);
            BoardLocationList options = WhereCanIReach(p, false);
            if (options.ContainsTheLocation(pVictim.Location))
                return true;

            return false;
        }


        bool InCheck(ColorsEnum c)
        {
            // if any piece can reach me, I'm in check.
            PlacedPiece king = FindPiece(PiecesEnum.King, c);
            foreach (PlacedPiece p in placedPieces)
            {
                //                if (CanAttack(p, king))
                if (CanAttackNowNoRecurse(p, king))
                    return true;
            }
            return false;
        }

        // a shallow check sees where i can reach without regard to whetehr it puts me into check doing it.
        // (a king can't end up in a position where i can reach it, even if me doing so is prevented cuz the move would put me in check)

        BoardLocationList WhereCanIReach(PlacedPiece p, bool fShallowCheck = true)
        {
            switch (p.PieceType)
            {
                case PiecesEnum.Knight:
                    {
                        BoardLocationList options = KnightStatic.CouldGoIfOmnipotent(p.Location);
                        options = YankSpotsThatArentBoardSpots(options);
                        options = YankSpotsOfThisColor(options, p.Color);
                        if(fShallowCheck)
                            options = YankSpotsThatPutMeInCheck(options, p);
                        return options;
                    }

                case PiecesEnum.King:
                    {
                        // THE POSITION OF THE QUEEN IS TECHNICALLY A POTENTIAL DESTINATION OF THE KING
                        // HOWEVER, THIS CODE WON'T REPORT IT, BECAUSE IT'S handled as a THEORETICAL BOARD
                        // RE-RUN elsewhere.
                        BoardLocationList spots = KingStatic.CouldGoIfOmnipotent(p.Location);
                        spots = YankSpotsThatArentBoardSpots(spots);
                        spots = YankSpotsOfThisColor(spots, p.Color); // diddily doo is not handled here!
                        if (fShallowCheck)
                            spots = YankSpotsThatPutMeInCheck(spots, p);
                        return spots;
                    }

                case PiecesEnum.Castle:
                    {
                        // radiating outward seems to make more sense.
                        // but how do i represent three directions?
                        // by changing q, changing r, and changing q&r
                        // really seeking six potential victims or same-teams.
                        // victims i can step on, same-team i cannot, but both stop my advance.
                        // +q, -q, +r, -r, +(q&r), -(q&r)
                        // just do six
                        BoardLocationList options = new BoardLocationList();

                        List<BoardLocationList> listOfRuns = CastleStatic.ListOfSequencesOfSpots(p.Location);
                        foreach (BoardLocationList ll in listOfRuns)
                        {
                            foreach (BoardLocation l in ll)
                            {
                                PlacedPiece pThere = AnyoneThere(l);
                                if (null == pThere)
                                {
                                    options.Add(l);
                                    continue;
                                }
                                if (pThere.Color == p.Color)
                                    break; // yes! so i can't move here, and run is OVER.

                                options.Add(pThere.Location);
                                break; // run is over!
                            }

                        }

                        options = YankSpotsThatArentBoardSpots(options);
                        if(fShallowCheck)
                            options = YankSpotsThatPutMeInCheck(options, p);

                        return options;
                    }

                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        List<PieceEvent> EventsFromAMove(PlacedPiece p, BoardLocation spot)
        {
            List<PieceEvent> events = new List<PieceEvent>();

            events.Add(new PieceEvent(p, EventTypeEnum.Remove)); // I leave this spot

            // if anyone was standing where I've landed, they aren't anymore.
            PlacedPiece deadp = this.AnyoneThere(spot);
            if (null != deadp)
                events.Add(new PieceEvent(deadp, EventTypeEnum.Remove));

            // if I jump down the hole, i don't leave this turn standing there.
            // THIS IS NOW WRONG, BUT HOLE JUMPS SHOULD BE FIGURED OUT EARLIER I *THINK*
            Debug.Assert(spot.Q != 0 || spot.R != 0 || p.PieceType == PiecesEnum.King); // nobody EXCEPT A KING gets to jump down the hole.

            PlacedPiece pp = new PlacedPiece(p, spot); // constructor clones me but to a new spot
            events.Add(new PieceEvent(pp, EventTypeEnum.Add)); // I appear at this spot

            return events;
        }


        public List<List<PieceEvent>> WhatCanIDo(PlacedPiece p, bool fdiddilydooit = true)
        {
            List<List<PieceEvent>> allPotentialOutcomes = new List<List<PieceEvent>>();

            switch (p.PieceType)
            {
                case PiecesEnum.Knight: // no special moves, no pathfinding. easiest!
                    {
                        BoardLocationList spots = WhereCanIReach(p);
                        // we want events associated with each spot
                        foreach (BoardLocation spot in spots)
                        {
                            List<PieceEvent> events = EventsFromAMove(p, spot);
                            allPotentialOutcomes.Add(events);
                        }
                        return allPotentialOutcomes;
                    }

                case PiecesEnum.King:
                    {
                        if (true == fdiddilydooit) // Is this a "no diddily doo" iteration?
                        {
                            // Is the Queen next to the king?
                            PlacedPiece queen = FindPiece(PiecesEnum.Queen, p.Color);
                            if (null != queen)
                            {
                                // Is there a queen of my color on any spot I can reach?
                                BoardLocationList queenCouldBe = KingStatic.CouldGoIfOmnipotent(p.Location);
                                if (queenCouldBe.ContainsTheLocation(queen.Location))
                                {
                                    // swap those two pieces on theoretical game board 
                                    Board bTheoretical = new Board(this);
                                    PlacedPiece pNewKing = new PlacedPiece(p, queen.Location);
                                    bTheoretical.Add(pNewKing);
                                    bTheoretical.Add(new PlacedPiece(queen, p.Location));

                                    // DID I JUST PUT MYSELF INTO CHECK? CUZ I'M PRETTY SURE THAT'S PROHIBITED.
                                    Debug.Assert(false == bTheoretical.InCheck(pNewKing.Color));

                                    allPotentialOutcomes = bTheoretical.WhatCanIDo(pNewKing, false); // interate, just once
                                }
                            }
                        }
                        BoardLocationList spots = WhereCanIReach(p);
                        // did i just land on some unfortunate piece? cuz those are events.
                        foreach (BoardLocation spot in spots)
                        {
                            List<PieceEvent> events = EventsFromAMove(p, spot);

                            allPotentialOutcomes.Add(events);
                        }
                        return allPotentialOutcomes;
                    }

                case PiecesEnum.Castle: // seemingly straightforward straightnesses
                    {
                        BoardLocationList spots = WhereCanIReach(p);
                        // we want events associated with each spot
                        foreach (BoardLocation spot in spots)
                        {
                            List<PieceEvent> events = EventsFromAMove(p, spot);
                            allPotentialOutcomes.Add(events);
                        }
                        return allPotentialOutcomes;
                    }

                case PiecesEnum.Queen:
                    // King and queen have optional "free move" together.
                    // Must seek paths
                    Debug.Assert(false); break;

                case PiecesEnum.Pawn:
                    // can croass the board and xform!
                    // A pawn involved in a mob can trigger a second pawn move if exiting the mob
                    // different capture pattern vs motion
                    // also electric fence!
                    Debug.Assert(false); break;
            }

            return allPotentialOutcomes;
        }
    }

    class Game
    {
        ColorsEnum m_first;
        ColorsEnum m_second;
        ColorsEnum m_third;
        //        ColorsEnum m_current;

        public Game(ColorsEnum first, ColorsEnum second, ColorsEnum third)
        {
            //m_current = 
            m_first = first;
            m_second = second;
            m_third = third;
        }



        public List<ColorsEnum> NextThree
        {
            get
            {
                List<ColorsEnum> three = new List<ColorsEnum> { m_first, m_second, m_third };
                return three;
            }
        }

    }

    class Recommender
    {
    }


    class Program
    {
        public static void ShowBoard(Board b)
        {
            /*
            foreach (PlacedPiece p in b.PlacedPieces)
            {
                Console.WriteLine("{0} {1} {2} {3}", p.Color, p.PieceType, p.Location.Q, p.Location.R);
            }
            */

            Form1.ShowBoard(b.PlacedPieces);
        }

        static void Main(string[] args)
        {
            Form1.StartMeUp();
        }

        public static void HCMain()
        {
            Board b = new Board();

            /*

            PlacedPiece king = new PlacedPiece(PiecesEnum.King, ColorsEnum.White, 1, 0);
            b.Add(king);
            PlacedPiece kingRemove = new PlacedPiece(PiecesEnum.King, ColorsEnum.White, 1, 0);
            b.Remove(kingRemove); // not the same object!! Well could be. Doesn't have to be.
            b.Add(king);

            PlacedPiece knight = new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, -1, 0);
            b.Add(knight);

            Console.WriteLine("We look like this:");
            ShowBoard(b);

            List<List<PieceEvent>> options = b.WhatCanIDo(knight);

            foreach (List<PieceEvent> could in options)
            {
                Board bTheoretical = new Board(b); // clone!

                foreach (PieceEvent pe in could)
                {
                    switch (pe.Type)
                    {
                        case EventTypeEnum.Add:
                            bTheoretical.Add(pe.Regarding);
                            break;
                        case EventTypeEnum.Remove:
                            bTheoretical.Remove(pe.Regarding);
                            break;
                    }
                }
                Console.WriteLine("a knight move could result in this:");
                ShowBoard(bTheoretical);
            }

            Console.WriteLine();

            options = b.WhatCanIDo(king);

            foreach (List<PieceEvent> could in options)
            {
                Board bTheoretical = new Board(b); // clone!

                foreach (PieceEvent pe in could)
                {
                    switch (pe.Type)
                    {
                        case EventTypeEnum.Add:
                            bTheoretical.Add(pe.Regarding);
                            break;
                        case EventTypeEnum.Remove:
                            bTheoretical.Remove(pe.Regarding);
                            break;
                    }
                }
                Console.WriteLine("a king move could result in this:");
                ShowBoard(bTheoretical);
            }

            Console.WriteLine();

            */
            
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Black, -1, -4));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Black, -4, -1));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Black, -1, -3));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Black, -2, -2));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Black, -3, -1));
            b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.Black, -2, -3));

            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Tan, -4, 5));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Tan, -1, 5));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Tan, -3, 4));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Tan, -2, 4));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Tan, -1, 4));
            b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.Tan, -3, 5));

            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, -4));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, -1));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, 4, -3));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, 4, -2));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, 4, -1));
            b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.White, 5, -3));

            ShowBoard(b);

            /*
            foreach (List<PieceEvent> could in options)
            {
                Board bTheoretical = new Board(b); // clone!

                foreach (PieceEvent pe in could)
                {
                    switch (pe.Type)
                    {
                        case EventTypeEnum.Add:
                            bTheoretical.Add(pe.Regarding);
                            break;
                        case EventTypeEnum.Remove:
                            bTheoretical.Remove(pe.Regarding);
                            break;
                    }
                }
                Console.WriteLine("a castle move could result in this:");
                ShowBoard(bTheoretical);
            }
            */


            // for each piece, see what it can do.
            // and for each outcome, see what the next player can do
            // and for each outcome, see what the third player can do
            // this is already insane. we'll see.
            // bredth first then it is.

            // throw kings on there
            //            PlacedPiece whiteKing = new PlacedPiece(PiecesEnum.King, ColorsEnum.White, -3, -1);
            // already there:            b.Add(whiteKing);

            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////

            Game g = new Game(ColorsEnum.White, ColorsEnum.Black, ColorsEnum.Tan);
            foreach (ColorsEnum color in g.NextThree)
            {
                Dictionary<PlacedPiece, List<List<PieceEvent>>> everyOption = new Dictionary<PlacedPiece, List<List<PieceEvent>>>();

                // for each piece of this color, what can it cause?
                foreach (PlacedPiece p in b.PlacedPiecesThisColor(color))
                {
                    // so yeah each piece is a unique key to a set of possible outcomes.
                    // even pawns who do an escape move?
                    // yeah... uh, yeah. any pawn could be the trigger piece... what matters is the board outcome.

                    List<List<PieceEvent>> myoptions = b.WhatCanIDo(p);
                    everyOption.Add(p, myoptions);
                }
                Dictionary<PlacedPiece, List<PieceEvent>> myCaptureChoices = new Dictionary<PlacedPiece, List<PieceEvent>>();
                foreach (var li in everyOption)
                {
                    // they key is the piece. the list for that key is what moves that piece can do.
                    // so tell me if any moves cause a capture.
                    foreach (var mov in li.Value)
                    {
                        // just look for three events, cuz one would be a remove.
                        if (mov.Count == 3)
                        {
                            // what do we remove?
                            foreach (var who in mov)
                            {
                                // if it's the key, we don't care about it.
                                if (who.Regarding.PieceType == li.Key.PieceType)
                                    if (who.Regarding.Color == li.Key.Color)
                                        continue;

                                // ok it's a capture. let's associate this move with the capture
                                Console.WriteLine("{0} could take {1}", li.Key.Color.ToString() + li.Key.PieceType.ToString(), who.Regarding.Color.ToString() + who.Regarding.PieceType.ToString());
                                myCaptureChoices.Add(li.Key, mov);
                            }
                        }
                    }
                }

                // Just choose the first capture choice.
                // ridonk
            }

            ShowBoard(b);

            foreach (PlacedPiece p in b.PlacedPieces)
            {
            }

            // should i do a test harness? Hell yeah.

            // Can I attack into the center, with both my own piece and the center piece falling?
            // Can I attack a piece and cause a regeneration of my own piece based on this event?

            // Put a king and a queen side-by-side, and see diddily-doos.

            // Pawns can step out of a group, the mob it's called
            // make a mob and watch it generate many (96?) breakout positions.
            // That would affirm Stephan's research.
            /*
            PlacedPiece pawn1 = new PlacedPiece(PiecesEnum.Pawn, ColorsEnum.White, 4, 0);
            PlacedPiece pawn2 = new PlacedPiece(PiecesEnum.Pawn, ColorsEnum.White, 3, 0);
            PlacedPiece pawn3 = new PlacedPiece(PiecesEnum.Pawn, ColorsEnum.White, 3, -1);
            b.Add(pawn1);
            b.Add(pawn2);
            b.Add(pawn3);

            List<List<PieceEvent>> pawnOptions = b.WhatCanIDo(pawn1);
            // I can test harness this sorta somehow:
            if( pawnOptions.Count() == 94)
            {
                // yay
            }
            */
        }
    }
}

// pants
