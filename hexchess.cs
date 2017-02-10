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
    // let's change them to pointy-topped! (geez, i hope i did this.)
    // http://www.redblobgames.com/grids/hexagons/


    class PawnStatic : PieceStatic
    {
        public static BoardLocationList CouldGoIfOmnipotent(BoardLocation fromHere) // SAME AS KING
        {
            int[,] JumpOptions = {
                { 0, -1 }, { 1, -1 }, {1, 0 }, {0,1 }, {-1, 1 }, {-1,0 } 
            };
            return CookUpLocations(fromHere, JumpOptions);
        }
    }

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

        static public bool IsSameLocation(BoardLocation one, BoardLocation two)
        {
            if (one.Q == two.Q)
                if (one.R == two.R)
                    return true;

            return false;
        }

        public bool IsPortal
        {
            get
            {
                if (q == 0 && r == 0)
                    return true;
                return false;
            }
        }

        public int Q { get { return q; } }
        public int R { get { return r; } }

        int q;
        int r;
    }

    public class BoardLocationList : List<BoardLocation>
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

    public class PieceList : List<Piece>
    {
        /*
        public void RemoveThePiece(PiecesEnum pt, ColorsEnum c)
        {
            foreach (var item in this)
            {
                if (item.PieceType == pt)
                    if (item.Color == c)
                    {
                        this.Remove(item);
                        return;
                    }

                Debug.Assert(false); // why would we ask for a piece that isn't in this set?
            }
        }
        */

        public bool ContainsThePiece(PiecesEnum pt, ColorsEnum c)
        {
            foreach( var item in this )
            {
                if (item.PieceType == pt)
                    if (item.Color == c)
                        return true;
            }
            return false;
        }
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
        PieceList sidelined = new PieceList();

        BoardLocationList highlightedSpots = new BoardLocationList(); // purely cosmetic
        public BoardLocationList HighlightedSpots { get { return highlightedSpots; } }


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
        public void Add(Piece p) // sidelined piece on board for color
        {
            sidelined.Add(p);
        }

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

                case PiecesEnum.Queen:
                    Debug.Assert(null == this.FindPiece(PiecesEnum.Queen, p.Color));
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

        public void Highlight(BoardLocation loc )
        {
            // we just record these locations and flash a designator on them
            highlightedSpots.Add(loc);
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

        BoardLocationList YankSpotsThatArentBoardSpots(BoardLocationList options, bool isAKing = false)
        {
            BoardLocationList realOptions = new BoardLocationList();

            foreach (BoardLocation b in options)
            {
                if (b.IsValidLocation())
                {
                    if (b.IsPortal)
                        if (false == isAKing)
                            continue; // zero zero is only valid for a king.
                        
                    realOptions.Add(b);
                }
            }

            return realOptions;
        }

        BoardLocationList YankSpotsOccupiedByThisColor(BoardLocationList options, ColorsEnum? c) // can be NULL for ANY color
        {
            BoardLocationList realOptions = new BoardLocationList();

            foreach (BoardLocation b in options)
            {
                PlacedPiece p = AnyoneThere(b);
                if (null != p)          // there's a piece
                    if((c == null) ||   // there's a piece and i yank pieces of any color
                        (p.Color != c)) // there's a piece of a color other than the one specified
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
                Board bHypothetical = new Board(this);       // clone meeeee
                bHypothetical.Remove(p);                     // take me off.
                bHypothetical.Add(new PlacedPiece(p, bl));   // put me on at the destination

                // I wanna see this board.
                Program.ShowBoard(bHypothetical);

                // WHAT IF THE KING MOVES INTO CHECK BUT ALSO MOVES TO 0,0 FOR THE WIN??
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
                        options = YankSpotsOccupiedByThisColor(options, p.Color);
                        if (fShallowCheck)
                            options = YankSpotsThatPutMeInCheck(options, p);
                        return options;
                    }

                case PiecesEnum.King:
                    {
                        // THE POSITION OF THE QUEEN IS TECHNICALLY A POTENTIAL DESTINATION OF THE KING
                        // HOWEVER, THIS CODE WON'T REPORT IT, BECAUSE IT'S handled as a THEORETICAL BOARD
                        // RE-RUN elsewhere.
                        BoardLocationList spots = KingStatic.CouldGoIfOmnipotent(p.Location);
                        spots = YankSpotsThatArentBoardSpots(spots, true); // true for the king!
                        spots = YankSpotsOccupiedByThisColor(spots, p.Color); // diddily doo is not handled here!
                        if (fShallowCheck)
                            spots = YankSpotsThatPutMeInCheck(spots, p);
                        return spots;
                    }

                case PiecesEnum.Pawn:
                    {
                        BoardLocationList stepSpots = PawnStatic.CouldGoIfOmnipotent(p.Location);
                        stepSpots = YankSpotsThatArentBoardSpots(stepSpots);
                        stepSpots = YankSpotsOccupiedByThisColor(stepSpots, null); // Yank spots occupied AT ALL

                        BoardLocationList attackWalkSpots = PawnStatic.WalkToSameColorStarSpots(this.PlacedPieces, p.Location);
                        attackWalkSpots = YankEmptySpots(attackWalkSpots);
                        attackWalkSpots = YankSpotsOccupiedByThisColor(attackWalkSpots, p.Color);

                        BoardLocationList allSpots = new BoardLocationList();
                        foreach (var spot in stepSpots)
                            allSpots.Add(spot);
                        foreach (var spot in attackWalkSpots)
                            allSpots.Add(spot);

                        if (fShallowCheck)
                            allSpots = YankSpotsThatPutMeInCheck(allSpots, p);

                        return allSpots;
                    }

                case PiecesEnum.Queen: // a queen is a castle plus other magic!
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
                                if (l.IsPortal)
                                    break; // run is over!

                                PlacedPiece pThere = AnyoneThere(l);
                                if (null == pThere)
                                {
                                    options.Add(l);
                                    continue;
                                }
                                if (pThere.Color == p.Color)
                                    break; // same-color piece, so i can't move here, and run is OVER.

                                options.Add(pThere.Location);
                                break; // run is over!
                            }
                        }

                        options = YankSpotsThatArentBoardSpots(options);
                        if (fShallowCheck)
                            options = YankSpotsThatPutMeInCheck(options, p);

                        if (PiecesEnum.Castle == p.PieceType)
                            return options;

                        // IF WE GET THIS FAR, THE PIECE IS A QUEEN.
                        m_QueenDesties = new BoardLocationList();
                        BoardLocationList soFar = new BoardLocationList();
                        soFar.Add(p.Location); // my first location is where I'm at
                        WalkToSameColorStarSpots(0, soFar);

                        foreach (BoardLocation b in m_QueenDesties)
                            options.Add(b);

                        return options;
                    }

                default:
                    Debug.Assert(false);
                    return null;
            }
        }


        BoardLocationList WhereCanQueenWalk(BoardLocation fromHere)
        {
            BoardLocationList destys = new BoardLocationList();

            int[,] WalkOptions = { { 1, -2 }, { 2, -1 }, { 1, 1 }, { -1, 2 }, { -2, 1 }, { -1, -1 } };
            int[,] RouteOne    = { { 0, -1 }, { 1, -1 }, { 1, 0 }, {  0, 1 }, { -1, 1 }, { -1,  0 } };
            int[,] RouteTwo    = { { 1, -1 }, { 1, 0 }, { 0, 1 },  { -1, 1 }, { -1, 0 }, {  0, -1 } };

            for (int iSet = 0; iSet < WalkOptions.GetLength(0); iSet++)
            {
                BoardLocation to = new BoardLocation(fromHere.Q + WalkOptions[iSet, 0], fromHere.R + WalkOptions[iSet, 1]);
                if (true == to.IsValidLocation())
                    if (null == this.AnyoneThere(to))
                    {
                        PlacedPiece one = this.AnyoneThere(new BoardLocation(fromHere.Q + RouteOne[iSet, 0], fromHere.R + RouteOne[iSet, 1]));
                        PlacedPiece two = this.AnyoneThere(new BoardLocation(fromHere.Q + RouteTwo[iSet, 0], fromHere.R + RouteTwo[iSet, 1]));
                        if(null == one || null == two )
                            destys.Add(to);
                    }
            }
            return destys;
        }

        public static BoardLocationList m_QueenDesties = new BoardLocationList();

    void WalkToSameColorStarSpots(int level, BoardLocationList pathSoFar)
        {
//            if (level == -1)
//                return;

            ++level;

            if (4 == level)
            {
                // on our third step, we ask: am i cool with these three steps?
                // 1. can't be any duplicates.

                int[,] Spots = { { 0, 1 }, { 0, 2 }, { 0, 3 }, { 1, 2 }, { 1, 3 }, { 2, 3 } };
                for(int iSet = 0; iSet < Spots.GetLength(0); iSet++)
                {
                    if (BoardLocation.IsSameLocation(pathSoFar[Spots[iSet, 0]], pathSoFar[Spots[iSet, 1]]))
                        return;
                }

                // 2. if no duplicates, then is this destination in our global list of available destinations?
                if (false == m_QueenDesties.ContainsTheLocation(pathSoFar[3]))
                    m_QueenDesties.Add(pathSoFar[3]);

//                level = -1;
                return;
            }

            foreach (var spot in WhereCanQueenWalk(pathSoFar[pathSoFar.Count - 1]))
            {
                pathSoFar.Add(spot);
                WalkToSameColorStarSpots(level, pathSoFar);
                pathSoFar.RemoveAt(pathSoFar.Count - 1);
            }

        }

    List<PieceEvent> EventsFromAMove(PlacedPiece p, BoardLocation spot)
        {
            List<PieceEvent> events = new List<PieceEvent>();

            events.Add(new PieceEvent(p, EventTypeEnum.Remove)); // I leave this spot

            // if anyone was standing where I've landed, they aren't anymore.
            // AND if any of my pieces of this type are sidelined, and nobody's blocking the portal, that piece appears at 0,0!
            PlacedPiece deadp = this.AnyoneThere(spot);
            if (null != deadp)
            {
                events.Add(new PieceEvent(deadp, EventTypeEnum.Remove));
                if(null != this.AnyoneThere(new BoardLocation(0,0))) // u blockin my portal?
                    if (sidelined.ContainsThePiece(deadp.PieceType, p.Color))
                    {
                    events.Add(new PieceEvent(new PlacedPiece(deadp.PieceType, p.Color, 0, 0), EventTypeEnum.Add));
                    // we don't remove it here!                    sidelined.RemoveThePiece (deadp.PieceType, p.Color);
                    }
            }

            // if my piece is in the portal, a move of any other piece causes my piece to return to sidelines.
            if(p.Location.IsPortal)
            {
                // i'm moving the piece from the portal... so there's no portal issue involving this move.
            }
            else
            {
                // i'm moving a piece that isn't in the portal.
                // so is there a piece in the portal?
                PlacedPiece portalJoe = this.AnyoneThere(new BoardLocation(0, 0));
                if(null != portalJoe)
                {
                    // is the portal piece same color as the mover of this move?
                    if(p.Color == portalJoe.Color)
                    {
                        // same color? then this piece dies in this move.
                        events.Add(new PieceEvent(portalJoe, EventTypeEnum.Remove));
                    }
                }
            }

            // if I jump down the hole, i don't leave this turn standing there.
            // THIS IS NOW WRONG, BUT HOLE JUMPS SHOULD BE FIGURED OUT EARLIER I *THINK*
            Debug.Assert(spot.Q != 0 || spot.R != 0 || p.PieceType == PiecesEnum.King); // nobody EXCEPT A KING gets to jump down the hole.

            PlacedPiece pp = new PlacedPiece(p, spot); // constructor clones me but to a new spot
            events.Add(new PieceEvent(pp, EventTypeEnum.Add)); // I appear at this spot

            return events;
        }

        // A Diddily Doo flips queen-king if possible, and determines what the piece can cause in both scenarios.
        // which might just be a swapped queen-king.

        public List<List<PieceEvent>> WhatCanICauseWithDoo(PlacedPiece p)
        {
            // First gather events with no diddily-doo.
            List<List<PieceEvent>> allPotentialOutcomes = WhatCanICause(p);

            // Now see if there's also a sequence of events after diddily-doo
            // Is the Queen next to the king?
            PlacedPiece king = FindPiece(PiecesEnum.King, p.Color);
            PlacedPiece queen = FindPiece(PiecesEnum.Queen, p.Color);
            if(null != queen)
            { 
                // shit there are some rules about moving into check. just be super-simple and we'll figure it out later.
                BoardLocationList queenCouldBe = KingStatic.CouldGoIfOmnipotent(king.Location);
                if(queenCouldBe.ContainsTheLocation(queen.Location))
                {
                    // swap those two pieces on theoretical game board 
                    Board bAfterDooSwap = new Board(this);
                    bAfterDooSwap.Remove(queen);
                    bAfterDooSwap.Remove(king);
                    var newKing = new PlacedPiece(PiecesEnum.King, queen.Color, queen.Location.Q, queen.Location.R);
                    var newQueen = new PlacedPiece(PiecesEnum.Queen, king.Color, king.Location.Q, king.Location.R);
                    bAfterDooSwap.Add(newKing);
                    bAfterDooSwap.Add(newQueen);

                    // We can (hey, probably) swap king-queen.
                    // but if this inquiry is about one of them, then we're inquiring about a piece in a different spot.
                    if (p.PieceType == PiecesEnum.King)
                        p = newKing;
                    if (p.PieceType == PiecesEnum.Queen)
                        p = newQueen;

                    List<List<PieceEvent>> evenMorePotentialOutcomes = bAfterDooSwap.WhatCanICause(p);

                    // each event set has one additional set, a swapparoo
                    // though i bet i'm going to be missing check cases and crying here.
                    foreach(var set in evenMorePotentialOutcomes)
                    {
                        set.Add(new PieceEvent(queen, EventTypeEnum.Remove));
                        set.Add(new PieceEvent(king, EventTypeEnum.Remove));
                        set.Add(new PieceEvent(newQueen, EventTypeEnum.Add));
                        set.Add(new PieceEvent(newKing, EventTypeEnum.Add));
                        allPotentialOutcomes.Add(set);
                    }
                    evenMorePotentialOutcomes = null;
                }
            }
            return allPotentialOutcomes;
        }

        // Front door for asking "hey, what outcomes can I produce if it's my turn to move now?"
        // Result is a list of a list of changes, one list per scenario.

        protected List<List<PieceEvent>> WhatCanICause(PlacedPiece p) // , bool fdiddilydooit = true)
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
                    {
                        // King and queen have optional "free move" together.
                        // Must seek paths
                        BoardLocationList spots = WhereCanIReach(p);

                        // we want events associated with each spot
                        foreach (BoardLocation spot in spots)
                        {
                            List<PieceEvent> events = EventsFromAMove(p, spot);
                            allPotentialOutcomes.Add(events);
                        }
                        return allPotentialOutcomes;
                    }

                case PiecesEnum.Pawn:
                    {
                        // 2. can croass the board and xform... into any piece?!
                        // 3. Exiting a mob requires two pawn moves.
                        // 4. electric fence!

                        // are we a raw lone pawn? for now assume yes.
                        BoardLocationList spots = WhereCanIReach(p); 

                        // IF I REACH ACROSS THE BOARD, I TRANSFORM, DUDE.
                        // (this requires establishment of a back/front wall for each color)

                        // we want events associated with each spot
                        foreach (BoardLocation spot in spots)
                        {
                            List<PieceEvent> events = EventsFromAMove(p, spot);
                            allPotentialOutcomes.Add(events);
                        }
                        return allPotentialOutcomes;

                    }
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
             Form1.ShowBoard(b.PlacedPieces, b.HighlightedSpots);
        }

        public static void FlashSpots(Board b, PlacedPiece p, List<List<PieceEvent>> options)
        {
            // Hmm, show a piece in red or something, and red circle or something everywhere they can reach.
            Board bFlashy = new Board(b); // copy  the board into our flashy board
            bFlashy.Highlight(p.Location);
            foreach (var changes in options)
            {
                foreach (var change in changes)
                {
                    if (change.Type == EventTypeEnum.Add)
                    {
                        bFlashy.Highlight(change.Regarding.Location);
                    }
                }
            }

            // show the original board
            ShowBoard(b);
            //
            ShowBoard(bFlashy);

            
        }

        static void Main(string[] args)
        {
            Form1.StartMeUp();
        }

        public static void HCMain()
        {
            Board b = new Board();

            /*

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

            /*
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Black, -1, -4));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Black, -4, -1));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Black, -1, -3));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Black, -2, -2));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Black, -3, -1));
            */

            PlacedPiece ppq = new PlacedPiece(PiecesEnum.Queen, ColorsEnum.Black, -3, -2);
            b.Add(ppq);
            b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.Black, -2, -3));
            PlacedPiece ppawn = new PlacedPiece(PiecesEnum.Pawn, ColorsEnum.Black, -1, -1);
            b.Add(ppawn);

            /*
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Tan, -4, 5));
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.Tan, -1, 5));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Tan, -3, 4));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Tan, -2, 4));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.Tan, -1, 4));
            b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.Tan, -3, 5));

            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, -4)); // reincarnate
            b.Add(new PlacedPiece(PiecesEnum.King, ColorsEnum.White, 1, 0)); // victory tester!
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, 2, 1)); // test can knight jump into 0,0
//            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, 0)); // testing the hole
            b.Add(new PlacedPiece(PiecesEnum.Castle, ColorsEnum.White, 5, -1));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, 4, -3));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, 4, -2));
            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, 4, -1));
            b.Add(new Piece(PiecesEnum.Castle, ColorsEnum.White)); // I have a castle on the sidelines.
//            b.Add(new PlacedPiece(PiecesEnum.Knight, ColorsEnum.White, 2,1)); // test can knight jump into 0,0
            */

            ShowBoard(b);

            //            List<List<PieceEvent>> options = b.WhatCanICauseWithDoo(ppq);
            List<List<PieceEvent>> options = b.WhatCanICauseWithDoo(ppawn);

            ShowBoard(b);
            FlashSpots(b, ppawn, options);

            // from here, we only know Game, not Board.


        // for each piece, see what it can do.
        // and for each outcome, see what the next player can do
        // and for each outcome, see what the third player can do
        // this is already insane. we'll see.
        // bredth first then it is.

        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////

        // Game knows the sequence of turns

        /*
        Game g = new Game(ColorsEnum.White, ColorsEnum.Black, ColorsEnum.Tan);

        int iNextOppoMax;
        int iOppoAfterThatMax;
        int iMyGuaranteedMin;
        TellMeOptionsWithTwoMaxesOneGuaranteedMin(g, b, out iNextOppoMax, out iOppoAfterThatMax, out iMyGuaranteedMin);

        for (int iTurnsIntoFuture = 0; iTurnsIntoFuture < 3; iTurnsIntoFuture++)
        {
            foreach (ColorsEnum color in g.NextThree)
            {
                Dictionary<PlacedPiece, List<List<PieceEvent>>> everyOption = new Dictionary<PlacedPiece, List<List<PieceEvent>>>();
                // HEY, IT'S JUST NOT OK TO REMEMBER WHICH PIECE CAUSED THE EVENTS.
                // well, it is ok. it's just not critical.

                // for each piece of this color, what can it cause?
                foreach (PlacedPiece p in b.PlacedPiecesThisColor(color))
                {
                    // so yeah each piece is a unique key to a set of possible outcomes.
                    // even pawns who do an escape move?
                    // yeah... uh, yeah. any pawn could be the trigger piece... what matters is the board outcome.

                    List<List<PieceEvent>> myoptions = b.WhatCanIDo(p);
                    FlashSpots(b, p, myoptions);
                    // This will need to be one tree, because we're saying throw out the intermediate steps?
                    // re-create them all, i mean???
                    // each time? like this? 
                    // this is going to be slow, might be painfully slow.
                    // But we can probably think of it this way first.
                    List<int> myGuaranteedMins = MyGuaranteedMins(b, color, myoptions);
                    List<int> nextPlayersPotentialMaxes = NextPlayersPotentialMaxes(b, g.After(color), myoptions);
                    List<int> thirdPlayersPotentialMaxes = ThirdPlayersPotentialMaxes(b, g.After(g.After(color)), myoptions);
                    Debug.Assert(myoptions.Count() == myGuaranteedMins.Count() == nextPlayersPotentialMaxes.Count() == thirdPlayersPotentialMaxes.Count());
                    everyOption.Add(p, myoptions, myGuaranteedMins, nextPlayersPotentialMaxes, thirdPlayersPotentialMaxes);
                }

                // My team's options, analyzed
                Dictionary<PlacedPiece, List<PieceEvent>> myMoveOptions = new Dictionary<PlacedPiece, List<PieceEvent>>();
                foreach (var li in everyOption)
                {
                    // they key is the piece. the list for that key is 21what moves that piece can do.
                    // so tell me if any moves cause a capture.
                    bool fKingWin = false;

                    foreach (var mov in li.Value)
                    {
                        // if king ends up in hole, we'll note that thanks
                        foreach (var scan in mov)
                        {
                            if (scan.Regarding.PieceType == PiecesEnum.King)
                                if (scan.Regarding.Location.IsPortal)
                                {
                                    fKingWin = true;
                                    break;
                                }
                        }
                        // just look >= three events, cuz one would be a remove.
                        // REMEMBER HERE WE'RE JUST TOYING WITH *ONLY* CONSIDERING CAPTURE SCENARIOS
                        // IN DEVELOPING AN AI. SOUNDS DOOMED. AND PIECE-UNMOVED OR PIECE-PORTALLED 
                        // MAKE THIS RESTRICTION MY MOVE COUNT DOOMED.
                        // don't we track every potential move?
                        // yes... BUT... i want to try this...
                        // i want a limited set of events to consider.

                        //                        if (mov.Count >= 3 || fKingWin)
                        // just fucking do this for every scenario,
                        {
                            string extraSnippet = "";
                            // what do we remove?
                            foreach (var who in mov)
                            {
                                // if it's on 0,0, give that piece a note.
                                if (fKingWin)
                                    extraSnippet = "King goes down portal for win.";
                                else
                                    if (who.Regarding.Location.IsPortal)
                                    extraSnippet = string.Format(", and gain a {0} at the portal, ", li.Key.PieceType.ToString());

                                // Other than popups in the portal, 
                                if (who.Regarding.PieceType == li.Key.PieceType)
                                    if (who.Regarding.Color == li.Key.Color)
                                        continue;

                                // ok it's a capture. let's associate this move with the capture
                                Console.Write("{0} could take {1}", li.Key.Color.ToString() + li.Key.PieceType.ToString(), who.Regarding.Color.ToString() + who.Regarding.PieceType.ToString());
                                myMoveOptions.Add(li.Key, mov);
                            }
                            Console.WriteLine(extraSnippet);
                        }
                    }
                }

                // I calculated all the options my pieces have.
                // now FOR EACH OPTION, i ask what the next player COULD do.
                // and i determine (a) the minimum GUARANTEED gain for me and (b) maximum POSSIBLE gain for the other playerS
                // so this set of myMoveOptions... would contain two or four numbers (a & b for my two opponents)
                // isn't there just ONE (a) and 2 (b)'s?
            }

            ShowBoard(b);
        }

        // starting with white for some reason, we've thought 3 moves ahead?!

        // Can I attack into the center, with both my own piece and the center piece falling?
        // Can I attack a piece and cause a regeneration of my own piece based on this event?

        // Put a king and a queen side-by-side, and see diddily-doos.

        // Pawns can step out of a group, the mob it's called
        // make a mob and watch it generate many (96?) breakout positions.
        // That would affirm Stephan's research.

*/
        }
}
}
