using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

public class Board
{
    public Board()
    {
        Init();
    }
    public Board(Board board)
    {
        _turn = board.getTurn();
        _check_mode = board.getCheckMode();
        _board = new Pieces[8, 8];
        for (int  i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                //_board[i, j] = new VoidPiece(i, j);
                _board[i, j] = board.get(i, j).DeepCopy();
            }
        }
    }
    
    public void Init()
    {
        if (_partie == 0)
        {
            _turn = true;
            _board = new Pieces[8, 8];
            for (int i = 2; i < 6; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    _board[i, j] = new VoidPiece(j, i);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                _board[1, i] = new Pawn(1, i);
                _board[6, i] = new Pawn(6, i, false);
            }
            _board[0, 0] = new Rook(0, 0);
            _board[0, 7] = new Rook(0, 7);
            _board[7, 0] = new Rook(7, 0, false);
            _board[7, 7] = new Rook(7, 7, false);
            _board[0, 1] = new Knight(0, 1);
            _board[0, 6] = new Knight(0, 6);
            _board[7, 1] = new Knight(7, 1, false);
            _board[7, 6] = new Knight(7, 6, false);
            _board[0, 2] = new Bishop(0, 2);
            _board[0, 5] = new Bishop(0, 5);
            _board[7, 2] = new Bishop(7, 2, false);
            _board[7, 5] = new Bishop(7, 5, false);
            _board[0, 3] = new Queen(0, 3);
            _board[7, 3] = new Queen(7, 3, false);
            _board[0, 4] = new King(0, 4);
            _board[7, 4] = new King(7, 4, false);
            _partie = 1;
            Console.WriteLine("La partie est initialisée");
        }
        else
        {
            Console.WriteLine("La partie est déjà initialisée");
        }
    }
    public Pieces get(int row, int column)
    {
        return _board[row, column];
    }
    public void move(Tuple<byte, byte> to_move_From,Tuple<byte, byte> to_move_on)
    {
        _check_mode = isCheck();
        _turn = !_turn;
        if(_board[to_move_From.Item1, to_move_From.Item2].getType() == typeOfPiece.King)
        {
            if(Math.Abs(to_move_From.Item2 - to_move_on.Item2) > 1)
            {
                get(to_move_From.Item1, to_move_From.Item2).unsetRoque();
                if (to_move_on.Item2 == 6)
                {
                    _board[to_move_From.Item1, 5] = _board[to_move_From.Item1, 7];
                    _board[to_move_From.Item1, 7] = new VoidPiece(to_move_From.Item1, 7);
                }
                else if (to_move_on.Item2 == 2)
                {
                    _board[to_move_From.Item1, 3] = _board[to_move_From.Item1, 0];
                    _board[to_move_From.Item1, 0] = new VoidPiece(to_move_From.Item1, 0);
                }
            }
        }
        _board[to_move_From.Item1, to_move_From.Item2].move(to_move_on);
        _board[to_move_on.Item1, to_move_on.Item2] = _board[to_move_From.Item1, to_move_From.Item2];
        _board[to_move_From.Item1, to_move_From.Item2] = new VoidPiece(to_move_From.Item1, to_move_From.Item2);
        if(_board[to_move_on.Item1, to_move_on.Item2].getType() == typeOfPiece.Pawn)
        {
            if (to_move_on.Item1 == 7)
            {
                _board[to_move_on.Item1, to_move_on.Item2] = new Queen(to_move_on.Item1, to_move_on.Item2, true);
            }
            else if (to_move_on.Item1 == 0)
            {
                _board[to_move_on.Item1, to_move_on.Item2] = new Queen(to_move_on.Item1, to_move_on.Item2, false);
            }
        }
        _check_mode = isCheck();
    }
    public bool getCheckMode()
    {
        return _check_mode;
    }
    public bool isVoid(int row, int column)
    {
        if (row < 0 || row > 7 || column < 0 || column > 7)
            return true;
        return (_board[row, column].getType() == typeOfPiece.voidPiece);
    }
    public bool isIn(int row, int column)
    {
        if (row < 0 || row > 7 || column < 0 || column > 7)
            return false;
        return true;
    }
    public bool getTurn()
    {
        return _turn;
    }
    public void setPossibilities(List<Tuple<byte, byte>> possibilities)
    {
        _all_moves = possibilities;
    }
    public void AllMoves(bool isWhiteMove)
    {
        _all_moves = new List<Tuple<byte, byte>>();

        for(int i = 0;  i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (_board[i, j].getType() != typeOfPiece.voidPiece && (_board[i, j].getType() != typeOfPiece.King))
                {
                    if (_board[i, j].isWhite() != isWhiteMove)
                    {
                        List<Tuple<byte, byte>> possible_cases = new List<Tuple<byte, byte>>();
                        if (_check_mode)
                        {
                            _board[i, j].getPossibilities(this, out possible_cases, true);
                        }
                        else
                        {
                            _board[i, j].getPossibilities(this, out possible_cases);
                        }
                        _all_moves.AddRange(possible_cases);
                    }
                }
            }
        }
    }
    public List<Tuple<byte, byte>> getMoves()
    {
        return _all_moves;
    }

    public bool isCheck()
    {
        AllMoves(_turn);
        Tuple<byte, byte> king = findKing(_turn);
        if (_all_moves.Contains(king))
        {
            return true;
        }
        return false;
    }

    public bool isCheckForKing(bool isWhite, Tuple<byte, byte> new_case)
    {
         AllMoves(isWhite);
        if(_all_moves.Contains(new_case))
        {
            return true;
        }
        return false;
     }
    public Tuple<byte, byte> findKing(bool isWhite)
    {
        for (byte i = 0; i < 8; i++)
        {
            for (byte j = 0; j < 8; j++)
            {
                if (_board[i, j].getType() == typeOfPiece.King && _board[i, j].isWhite() == isWhite)
                {
                    return new Tuple<byte, byte>(i, j);
                }
            }
        }
        return new Tuple<byte, byte>(0, 0);
    }
    public bool isCheck(bool isWhite)
    {
        AllMoves(isWhite);
        Tuple<byte, byte> new_case = new Tuple<byte, byte>(0, 0);
        new_case = findKing(isWhite);
        if (_all_moves.Contains(new_case))
        {
            return true;
        }
        return false;

    }
    public void Reset()
    {
        _board = new Pieces[8, 8];
        _partie = 0;
    }

    private List<Tuple<byte, byte>> _all_moves;
    private int _partie = 0;
    public Pieces[,] _board;
    public bool _turn;
    public bool _check_mode;
}