using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
public enum typeOfPiece{voidPiece,
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
};
public class Pieces
{
    protected typeOfPiece _type_piece;
    protected int _caseV;
    protected int _caseH;
    protected int[] _possible_cases;
    protected Board _board;
    protected bool _isWhite;
    public Pieces()
    {
        _type_piece = typeOfPiece.voidPiece;
    }
    public virtual void unsetRoque()
    {
    }

    public virtual Pieces DeepCopy()
    {
        Pieces copy = new Pieces(_caseV, _caseH);
        copy._type_piece = _type_piece;
        copy._isWhite = _isWhite;
        return copy;
    }
    public Pieces(int caseV, int caseH)
    {
        _type_piece = typeOfPiece.voidPiece;
        _caseV = caseV;
        _caseH = caseH;
    }
    public typeOfPiece getType()
    {
        return _type_piece;
    }
    public bool isWhite()
    {
        return _isWhite;
    }
    public virtual void getPossibilities(Board board, out List<Tuple<byte, byte>> possible_cases, bool protected_mode = false)
    {
        possible_cases = new List<Tuple<byte, byte>>();
    }
    public virtual void move(Tuple<byte, byte> on)
    {
        _caseV = on.Item1;
        _caseH = on.Item2;
    }
    public virtual List<Tuple<byte, byte>> getUncheckPossibilities(ref Board board, Tuple<byte, byte> from)
    {
        List<Tuple<byte, byte>> possible_cases = new List<Tuple<byte, byte>>();
        foreach (Tuple<byte, byte> tuple in board.getMoves())
        {
            Board copy = new Board(board);
            copy.move(from, tuple);
            if (!copy.isCheck(_isWhite))
            {
                possible_cases.Add(tuple);
            }
        }
        return possible_cases;
    }
}
internal class VoidPiece : Pieces
{
    public VoidPiece()
    {
        _type_piece = typeOfPiece.voidPiece;
    }
    public VoidPiece(int caseV, int caseH)
    {
        _type_piece = typeOfPiece.voidPiece;
        _caseV = caseV;
        _caseH = caseH;
    }
    public override VoidPiece DeepCopy()
    {
        VoidPiece copy = new VoidPiece(_caseV, _caseH);
        return copy;
    }
}
public class Pawn : Pieces
{
    protected int _count = 0;
    private bool _isFirst = true;
    public Pawn()
    {
        _type_piece = typeOfPiece.Pawn;
    }
    public Pawn(int caseV, int caseH, bool isWhite = true)
    {
        _type_piece = typeOfPiece.Pawn;
        _caseV = caseV; //row
        _caseH = caseH; //column
        _isWhite = isWhite;
    }
    public override Pawn DeepCopy()
    {
        Pawn copy = new Pawn(_caseV, _caseH, _isWhite);
        copy._count = _count;
        return copy;
    }
    public override void getPossibilities(Board board, out List<Tuple<byte, byte>> possible_cases, bool protected_mode = false)
    {
        possible_cases = new List<Tuple<byte, byte>>();
        int direction = 2 * Convert.ToInt32(_isWhite) - 1;
        if (_caseV + direction == 8 || _caseV + direction == -1) //should never happen because of promotion but we never know
        { 
            return; 
        }
        if(board.isVoid(_caseV + direction, _caseH))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + direction), (byte)_caseH));
            if (_isFirst && board.isVoid(_caseV + 2 * direction, _caseH))
            {
                possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + 2 * direction), (byte)_caseH));
            }
        }
        if (!board.isVoid(_caseV + direction, _caseH + 1) && board.get(_caseV + direction, _caseH + 1).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + direction ), (byte)(_caseH + 1)));
        }
        if (!board.isVoid(_caseV + direction, _caseH - 1) && board.get(_caseV + direction, _caseH - 1).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + direction), (byte)(_caseH - 1)));
        }
    }
    public override void move(Tuple<byte, byte> on)
    {
        _isFirst = false;
        _caseV = on.Item1;
        _caseH = on.Item2;

    }
}
internal class Rook : Pieces
{
    protected int _count = 0;
    public Rook()
    {
        _type_piece = typeOfPiece.Rook;
    }
    public Rook(int caseV, int caseH, bool isWhite = true)
    {
        _type_piece = typeOfPiece.Rook;
        _caseV = caseV;
        _caseH = caseH;
        _isWhite = isWhite;
    }
    public override Rook DeepCopy()
    {
        Rook copy = new Rook(_caseV, _caseH, _isWhite);
        copy._count = _count;
        return copy;

    }
    public override void getPossibilities(Board board, out List<Tuple<byte, byte>> possible_cases, bool protected_mode =false)
    {
        possible_cases = new List<Tuple<byte, byte>>();
        int i = 1;
        while (board.isVoid(_caseV + i, _caseH) && board.isIn(_caseV + i, _caseH))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)_caseH));
            i++;
        }
        if (!board.isVoid(_caseV + i, _caseH) && board.isIn(_caseV + i, _caseH) && board.get(_caseV + i, _caseH).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)_caseH));
        }
        if (!board.isVoid(_caseV + i, _caseH) && board.isIn(_caseV + i, _caseH) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)_caseH));
        }

        i = 1;
        while (board.isVoid(_caseV - i, _caseH) && board.isIn(_caseV - i, _caseH))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)_caseH));
            i++;
        }
        if (!board.isVoid(_caseV - i, _caseH) && board.isIn(_caseV - i, _caseH) && board.get(_caseV - i, _caseH).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)_caseH));
        }
        if (!board.isVoid(_caseV - i, _caseH) && board.isIn(_caseV - i, _caseH) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)_caseH));
        }
        i = 1;
        while (board.isVoid(_caseV, _caseH + i) && board.isIn(_caseV, _caseH + i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH + i)));
            i++;
        }
        if (!board.isVoid(_caseV, _caseH + i) && board.isIn(_caseV, _caseH + i) && board.get(_caseV, _caseH + i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH + i)));
        }
        if (!board.isVoid(_caseV, _caseH + i) && board.isIn(_caseV, _caseH + i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH + i)));
        }

        i = 1;
        while (board.isVoid(_caseV, _caseH - i) && board.isIn(_caseV, _caseH - i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH - i)));
            i++;
        }
        if (!board.isVoid(_caseV, _caseH - i) && board.isIn(_caseV, _caseH - i) && board.get(_caseV, _caseH - i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH - i)));
        }
        if (!board.isVoid(_caseV, _caseH - i) && board.isIn(_caseV, _caseH - i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH - i)));
        }

    }

}
    internal class Knight : Pieces
{
    protected int _count = 0;
    public Knight()
    {
        _type_piece = typeOfPiece.Knight;
    }
    public override Knight DeepCopy()
    {
        Knight copy = new Knight(_caseV, _caseH, _isWhite);
        copy._count = _count;
        return copy;

    }
    public Knight(int caseV, int caseH, bool isWhite = true)
    {
        _type_piece = typeOfPiece.Knight;
        _caseV = caseV;
        _caseH = caseH;
        _isWhite = isWhite;
    }
    public override void getPossibilities(Board board, out List<Tuple<byte, byte>> possible_cases, bool protected_mode = false)
    {
        possible_cases = new List<Tuple<byte, byte>>();
        for (int i = -2; i < 3; i++)
        {
            byte caseV = (byte)(_caseV + i);
            byte increment = (byte)(3 - Math.Abs(i));
            byte caseH = (byte)(_caseH + increment);
            if (board.isIn(caseV, caseH) && (board.isVoid(caseV, caseH) || board.get(caseV, caseH).isWhite() != _isWhite))
            {
                possible_cases.Add(new Tuple<byte, byte>(caseV, caseH));
            }
            if (board.isIn(caseV, caseH) && board.isVoid(caseV, caseH) && protected_mode)
            {
                possible_cases.Add(new Tuple<byte, byte>(caseV, caseH));
            }
            caseH = (byte)(_caseH - increment);
            if (board.isIn(caseV, caseH) && (board.isVoid(caseV, caseH) || board.get(caseV, caseH).isWhite() != _isWhite))
            {
                possible_cases.Add(new Tuple<byte, byte>(caseV, caseH));
            }
            if (board.isIn(caseV, caseH) && board.isVoid(caseV, caseH) && protected_mode)
            {
                possible_cases.Add(new Tuple<byte, byte>(caseV, caseH));
            }

        }
    }
}
internal class Bishop : Pieces
{
    protected int _count = 0;
    public Bishop()
    {
        _type_piece = typeOfPiece.Bishop;
    }
    public Bishop(int caseV, int caseH, bool isWhite = true)
    {
        _type_piece = typeOfPiece.Bishop;
        _caseV = caseV;
        _caseH = caseH;
        _isWhite = isWhite;
    }
    public override Bishop DeepCopy()
    {
        Bishop copy = new Bishop(_caseV, _caseH, _isWhite);
        copy._count = _count;
        return copy;

    }
    public override void getPossibilities(Board board, out List<Tuple<byte, byte>> possible_cases, bool protected_mode = false)
    {
        possible_cases = new List<Tuple<byte, byte>>();
        int i = 1;
        while (board.isVoid(_caseV + i, _caseH + i) && board.isIn(_caseV + i, _caseH + i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH + i)));
            i++;
        }
        if (!board.isVoid(_caseV + i, _caseH + i) && board.isIn(_caseV + i, _caseH + i) && board.get(_caseV + i, _caseH + i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH + i)));
        }
        if (protected_mode && !board.isVoid(_caseV + i, _caseH + i) && board.isIn(_caseV + i, _caseH + i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH + i)));
        }
        i = 1;
        while (board.isVoid(_caseV - i, _caseH - i) && board.isIn(_caseV - i, _caseH - i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH - i)));
            i++;
        }
        if (!board.isVoid(_caseV - i, _caseH - i) && board.isIn(_caseV - i, _caseH - i) && board.get(_caseV - i, _caseH - i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH - i)));
        }
        if (!board.isVoid(_caseV - i, _caseH - i) && board.isIn(_caseV - i, _caseH - i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH - i)));
        }

        i = 1;
        while (board.isVoid(_caseV - i, _caseH + i) && board.isIn(_caseV - i, _caseH + i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH + i)));
            i++;
        }
        if (!board.isVoid(_caseV - i, _caseH + i) && board.isIn(_caseV - i, _caseH + i) && board.get(_caseV - i, _caseH + i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH + i)));
        }
        if (!board.isVoid(_caseV - i, _caseH + i) && board.isIn(_caseV - i, _caseH + i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH + i)));
        }

        i = 1;
        while (board.isVoid(_caseV + i, _caseH - i) && board.isIn(_caseV + i, _caseH - i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH - i)));
            i++;
        }
        if (!board.isVoid(_caseV + i, _caseH - i) && board.isIn(_caseV + i, _caseH - i) && board.get(_caseV + i, _caseH - i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH - i)));
        }
        if (!board.isVoid(_caseV + i, _caseH - i) && board.isIn(_caseV + i, _caseH - i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH - i)));
        }
    }
}
    internal class Queen : Pieces
{
    protected int _count = 0;
    public Queen()
    {
        _type_piece = typeOfPiece.Queen;
    }
    public Queen(int caseV, int caseH, bool isWhite = true)
    {
        _type_piece = typeOfPiece.Queen;
        _caseV = caseV;
        _caseH = caseH;
        _isWhite = isWhite;
    }
    public override void getPossibilities(Board board, out List<Tuple<byte, byte>> possible_cases, bool protected_mode = false)
    {
        possible_cases = new List<Tuple<byte, byte>>();
        int i = 1;
        while (board.isVoid(_caseV + i, _caseH + i) && board.isIn(_caseV + i, _caseH + i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH + i)));
            i++;
        }
        if (!board.isVoid(_caseV + i, _caseH + i) && board.isIn(_caseV + i, _caseH + i) && board.get(_caseV + i, _caseH + i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH + i)));
        }
        if (protected_mode && !board.isVoid(_caseV + i, _caseH + i) && board.isIn(_caseV + i, _caseH + i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH + i)));
        }
        i = 1;
        while (board.isVoid(_caseV - i, _caseH - i) && board.isIn(_caseV - i, _caseH - i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH - i)));
            i++;
        }
        if (!board.isVoid(_caseV - i, _caseH - i) && board.isIn(_caseV - i, _caseH - i) && board.get(_caseV - i, _caseH - i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH - i)));
        }
        if (!board.isVoid(_caseV - i, _caseH - i) && board.isIn(_caseV - i, _caseH - i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH - i)));
        }

        i = 1;
        while (board.isVoid(_caseV - i, _caseH + i) && board.isIn(_caseV - i, _caseH + i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH + i)));
            i++;
        }
        if (!board.isVoid(_caseV - i, _caseH + i) && board.isIn(_caseV - i, _caseH + i) && board.get(_caseV - i, _caseH + i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH + i)));
        }
        if (!board.isVoid(_caseV - i, _caseH + i) && board.isIn(_caseV - i, _caseH + i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH + i)));
        }

        i = 1;
        while (board.isVoid(_caseV + i, _caseH - i) && board.isIn(_caseV + i, _caseH - i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH - i)));
            i++;
        }
        if (!board.isVoid(_caseV + i, _caseH - i) && board.isIn(_caseV + i, _caseH - i) && board.get(_caseV + i, _caseH - i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)(_caseH - i)));
        }
        if (!board.isVoid(_caseV + i, _caseH - i) && board.isIn(_caseV + i, _caseH - i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)(_caseH - i)));
        }

        i = 1;
        while (board.isVoid(_caseV + i, _caseH) && board.isIn(_caseV + i, _caseH))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)_caseH));
            i++;
        }
        if (!board.isVoid(_caseV + i, _caseH) && board.isIn(_caseV + i, _caseH) && board.get(_caseV + i, _caseH).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)_caseH));
        }
        if (!board.isVoid(_caseV + i, _caseH) && board.isIn(_caseV + i, _caseH) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV + i), (byte)_caseH));
        }

        i = 1;
        while (board.isVoid(_caseV - i, _caseH) && board.isIn(_caseV - i, _caseH))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)_caseH));
            i++;
        }
        if (!board.isVoid(_caseV - i, _caseH) && board.isIn(_caseV - i, _caseH) && board.get(_caseV - i, _caseH).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)_caseH));
        }
        if (!board.isVoid(_caseV - i, _caseH) && board.isIn(_caseV - i, _caseH) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV - i), (byte)_caseH));
        }
        i = 1;
        while (board.isVoid(_caseV, _caseH + i) && board.isIn(_caseV, _caseH + i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH + i)));
            i++;
        }
        if (!board.isVoid(_caseV, _caseH + i) && board.isIn(_caseV, _caseH + i) && board.get(_caseV, _caseH + i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH + i)));
        }
        if (!board.isVoid(_caseV, _caseH + i) && board.isIn(_caseV, _caseH + i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH + i)));
        }

        i = 1;
        while (board.isVoid(_caseV, _caseH - i) && board.isIn(_caseV, _caseH - i))
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH - i)));
            i++;
        }
        if (!board.isVoid(_caseV, _caseH - i) && board.isIn(_caseV, _caseH - i) && board.get(_caseV, _caseH - i).isWhite() != _isWhite)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH - i)));
        }
        if (!board.isVoid(_caseV, _caseH - i) && board.isIn(_caseV, _caseH - i) && protected_mode)
        {
            possible_cases.Add(new Tuple<byte, byte>((byte)(_caseV), (byte)(_caseH - i)));
        }

    }
    public override Queen DeepCopy()
    {
        Queen copy = new Queen(_caseV, _caseH, _isWhite);
        copy._count = _count;
        return copy;

    }
}
    internal class King : Pieces
{
    protected int _count = 0;
    private bool _rook = true;
    public bool getRook()
    {
        return _rook;
    }
    public override void unsetRoque()
    {
        _rook = false;
    }
    public King()
    {
        _type_piece = typeOfPiece.King;
    }
    public King(int caseV, int caseH, bool isWhite = true)
    {
        _type_piece = typeOfPiece.King;
        _caseV = caseV;
        _caseH = caseH;
        _isWhite = isWhite;
    }
    public override King DeepCopy()
    {
        King copy = new King(_caseV, _caseH, _isWhite);
        copy._count = _count;
        return copy;
    }
    public override void getPossibilities(Board board, out List<Tuple<byte, byte>> possible_cases, bool protected_mode = false)
    {
        possible_cases = new List<Tuple<byte, byte>>();
        for (int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if ((i != 0) || (j != 0))
                {
                    byte caseV = (byte)(_caseV + i);
                    byte caseH = (byte)(_caseH + j);
                    if (board.isIn(caseV, caseH) && (board.isVoid(caseV, caseH) || board.get(caseV, caseH).isWhite() != _isWhite))
                    {
                        Tuple<byte, byte> cases = new Tuple<byte, byte>(caseV, caseH);
                        if (!board.isCheckForKing(_isWhite, cases))
                        {
                            possible_cases.Add(cases);
                        }
                    }
                }
            }
        }
        if(_rook)
        {
            if (!_isWhite)
            {
                if (board.get(7, 0) != null && board.get(7, 0).getType() == typeOfPiece.Rook && board.get(7, 0).isWhite() == _isWhite)
                {
                    if (board.isVoid(7, 1) && board.isVoid(7, 2) && board.isVoid(7, 3))
                    {
                        if (!board.isCheckForKing(_isWhite, new Tuple<byte, byte>(7, 1)) && !board.isCheckForKing(_isWhite, new Tuple<byte, byte>(7, 2)))
                        {
                            possible_cases.Add(new Tuple<byte, byte>(7, 2));
                        }
                    }
                }
                if (board.get(7, 7) != null && board.get(7, 7).getType() == typeOfPiece.Rook && board.get(7, 7).isWhite() == _isWhite)
                {
                    if (board.isVoid(7, 5) && board.isVoid(7, 6))
                    {
                        if (!board.isCheckForKing(_isWhite, new Tuple<byte, byte>(7, 5)) && !board.isCheckForKing(_isWhite, new Tuple<byte, byte>(7, 6)))
                        {
                            possible_cases.Add(new Tuple<byte, byte>(7, 6));
                        }
                    }
                }
            }
            else
            {
                if (board.get(0, 0) != null && board.get(0, 0).getType() == typeOfPiece.Rook && board.get(0, 0).isWhite() == _isWhite)
                {
                    if (board.isVoid(0, 1) && board.isVoid(0, 2) && board.isVoid(0, 3))
                    {
                        if (!board.isCheckForKing(_isWhite, new Tuple<byte, byte>(0, 1)) && !board.isCheckForKing(_isWhite, new Tuple<byte, byte>(0, 2)))
                        {
                            possible_cases.Add(new Tuple<byte, byte>(0, 2));
                        }
                    }
                }
                if (board.get(0, 7) != null && board.get(0, 7).getType() == typeOfPiece.Rook && board.get(0, 7).isWhite() == _isWhite)
                {
                    if (board.isVoid(0, 5) && board.isVoid(0, 6))
                    {
                        if (!board.isCheckForKing(_isWhite, new Tuple<byte, byte>(0, 5)) && !board.isCheckForKing(_isWhite, new Tuple<byte, byte>(0, 6)))
                        {
                            possible_cases.Add(new Tuple<byte, byte>(0, 6));
                        }
                    }
                }
            }
        }
    }
}