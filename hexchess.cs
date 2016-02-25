using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HexC
{
    enum PiecesEnum { Pawn, Knight, Castle, Queen, King }
    enum ColorsEnum { White, Brown, Black }

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
            int[,] StarOne = { { 1,-1 }, { 2,-2 }, { 3, -3 }, { 4, -4 }, { 5, -5 } };
            int[,] StarTwo = { { 0, 1 }, { 0, 2 }, { 0,  3 }, { 0,  4 }, { 0,  5 } };
            int[,] StarThr = { { -1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 0 }, { 1, -1 } };
            int[,] StarFou = { { -1, 1 }, { -2, 2 }, { -3, 3 }, { -4, 4 }, { -5, 5 } };
            int[,] StarFiv = { { 0, 1 }, { 0, 2 }, { 0, 3 },  { 0, 4 },  { 0, 5 } };
            ll.Add(CookUpLocations(loc, StarOne));
            ll.Add(CookUpLocations(loc, StarTwo));
            ll.Add(CookUpLocations(loc, StarThr));
            ll.Add(CookUpLocations(loc, StarFou));
            ll.Add(CookUpLocations(loc, StarFiv));

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

    class BoardLocation
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
            if (Math.Abs(q) > 5) return false;
            if (Math.Abs(r) > 5) return false;
            return true;
        }

        public int Q { get { return q; } }
        public int R { get { return r; } }

        int q;
        int r;
    }

    class BoardLocationList : List<BoardLocation>
    {
        // i only keep you from shooting yourself. nothing more.

        public bool ContainsTheLocation(BoardLocation bToMatch)
        {
            foreach( BoardLocation bl in this )
            {
                if (bl.Q == bToMatch.Q)
                    if (bl.R == bToMatch.R)
                        return true;
            }
            return false;
        }
    }

    class Piece
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

    class PlacedPiece : Piece
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

        public bool DeepEquals( PlacedPiece p )
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
        public void Add(PlacedPiece p) { placedPieces.Add(p); }

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

            foreach( PlacedPiece p in placedPieces )
            {
                if (p.Color == col)
                    myCol.Add(p);
            }
            return myCol;
        }


        PlacedPiece AnyoneThere(BoardLocation b)
        {
            foreach( PlacedPiece pp in placedPieces )
            {
                if (pp.Location.Q == b.Q)
                    if (pp.Location.R == b.R)
                        return pp ;
            }
            return null;
        }

        BoardLocationList YankSpotsThatArentBoardSpots( BoardLocationList options )
        {
            BoardLocationList realOptions = new BoardLocationList();

            foreach( BoardLocation b in options )
            {
                if (b.IsValidLocation())
                    realOptions.Add(b);
            }

            return realOptions;
        }

        BoardLocationList YankSpotsOfThisColor(BoardLocationList options, ColorsEnum c)
        {
            BoardLocationList realOptions = new BoardLocationList();

            foreach ( BoardLocation b in options)
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

            foreach( PlacedPiece p in this.placedPieces)
            {
                if (p.PieceType == type)
                    if (p.Color == c)
                        return p;
            }
            return null;
        }

        BoardLocationList YankSpotsThatPutMeInCheck(BoardLocationList options, PlacedPiece p)
        {
            BoardLocationList realOptions = new BoardLocationList();

            foreach (BoardLocation bl in options)
            {
                // Make a hypothetical board
                Board bHypothetical = new Board(this);     // clone meeeee
                bHypothetical.Remove(p);                     // take me off.
                bHypothetical.Add(new PlacedPiece(p, bl));   // put me on at the destination

                if (false == bHypothetical.InCheck(p.Color)) // see if i'm in check
                    realOptions.Add(bl);
            }
            return realOptions;
        }

        bool CanAttack(PlacedPiece p, PlacedPiece pVictim)
        {
            if (p.Color == pVictim.Color)
                return false;// can't attack my kind.

            BoardLocationList options = WhereCanIReach(p);
            if (options.Contains(pVictim.Location))
                return true;

            return false;
        }

        bool InCheck(ColorsEnum c)
        {
            // if any piece can reach me, I'm in check.
            PlacedPiece king = FindPiece(PiecesEnum.King, c);
            foreach (PlacedPiece p in placedPieces)
            {
                if (CanAttack(p, king))
                    return true;
            }
            return false;
        }

        BoardLocationList WhereCanIReach ( PlacedPiece p )
        {
            switch(p.PieceType)
            {
                case PiecesEnum.Knight:
                    {
                        BoardLocationList options = KnightStatic.CouldGoIfOmnipotent(p.Location);
                        options = YankSpotsThatArentBoardSpots(options);
                        options = YankSpotsOfThisColor(options, p.Color);
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
                        foreach(BoardLocationList ll in listOfRuns)
                        {
                            foreach(BoardLocation l in ll)
                            {
                                PlacedPiece pThere = AnyoneThere(l);
                                if(null == pThere)
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
            if ((spot.Q != 0) || (spot.R != 0))
            {
                PlacedPiece pp = new PlacedPiece(p, spot); // constructor clones me but to a new spot
                events.Add(new PieceEvent(pp, EventTypeEnum.Add)); // I appear at this spot
            }
            else
            {
                Console.WriteLine("Hi");
            }
            return events;
        }



    public List<List<PieceEvent>> WhatCanIDo(PlacedPiece p, bool fdiddilydooit = true )
        {
            List<List<PieceEvent>> allPotentialOutcomes = new List<List<PieceEvent>>();

            switch (p.PieceType)
            {
                case PiecesEnum.Knight: // no special moves, no pathfinding. easiest!
                    {
                        BoardLocationList spots = WhereCanIReach(p);
                        // we want events associated with each spot
                        foreach (BoardLocation spot in spots )
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

            return allPotentialOutcomes ;
        }
    }

    class Game
    {
        ColorsEnum m_first;
        ColorsEnum m_second;
        ColorsEnum m_third;
//        ColorsEnum m_current;

        public Game( ColorsEnum first, ColorsEnum second, ColorsEnum third)
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
        static void ShowBoard(Board b)
        {
            foreach (PlacedPiece p in b.PlacedPieces)
            {
                Console.WriteLine("{0} {1} {2} {3}", p.Color, p.PieceType, p.Location.Q, p.Location.R);
            }
        }

        static void Main(string[] args)
        {
            Board b = new Board();

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

            PlacedPiece castle = new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Black, -2, 0);
            b.Add(castle);

            options = b.WhatCanIDo(castle);

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


            // for each piece, see what it can do.
            // and for each outcome, see what the next player can do
            // and for each outcome, see what the third player can do
            // this is already insane. we'll see.
            // bredth first then it is.

            // throw kings on there
            PlacedPiece whiteKing = new PlacedPiece(PiecesEnum.King, ColorsEnum.White, -3, -1);
            b.Add(whiteKing);
            PlacedPiece brownKing = new PlacedPiece(PiecesEnum.King, ColorsEnum.Brown, -4, -1);
            b.Add(brownKing);

            Game g = new Game(ColorsEnum.White, ColorsEnum.Black, ColorsEnum.Brown);
            foreach (ColorsEnum color in g.NextThree)
            {
                Dictionary<PlacedPiece, List<List<PieceEvent>>> everyOption = new Dictionary<PlacedPiece, List<List<PieceEvent>>>();

                // for each piece of this color, what can it cause?
                foreach ( PlacedPiece p in b.PlacedPiecesThisColor(color))
                {
                    // I want enough data to spawn next moves from a "possibleMove"
                    // I kinda want to know how "objectively" the future step differs from the current step
                    // as in points or something.
                    // so are we supposed to store this piece as they key to this move?
                    // well that seems to be how we're doing it here.
                    // each piece can result in potential outcomes.
                    // and i will need some kind of accounting of ideas here.
                    // so yeah each piece is a unique key to a set of possible outcomes.
                    // even pawns who do an escape move?
                    // yeah... uh, yeah. any pawn could be the trigger piece... what matters is the board outcome.
                    List<List<PieceEvent>> myoptions = b.WhatCanIDo(p);
                    everyOption.Add(p, myoptions);
                }
            }

            foreach( PlacedPiece p in b.PlacedPieces )
            {


            }

            // should i do a test harness?


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
