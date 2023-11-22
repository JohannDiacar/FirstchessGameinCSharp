using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessWPF
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static class Constants
        {
            public static readonly SolidColorBrush WHITEPIECESCOLOR = new SolidColorBrush(Colors.Beige);
            public static readonly SolidColorBrush BLACKPIECESCOLOR = new SolidColorBrush(Colors.Black);
            public static readonly SolidColorBrush WHITECASESCOLOR = new SolidColorBrush(Colors.White);
            public static readonly SolidColorBrush BLACKCASESCOLOR = new SolidColorBrush(Colors.DarkGray);
            public static readonly SolidColorBrush ACTIVECASE = new SolidColorBrush(Colors.DarkRed);
        }
        protected Board _board;
        protected bool _is_selected;
        protected List<Tuple<byte, byte>> _possible_cases;
        protected Tuple<byte, byte> _selected_case;
        public MainWindow()
        {
            _is_selected = false;
            _board = new Board();
            InitializeComponent();
            FillGridWithChessPatern();
            FillChessPieces(ref _board);
        }
        private void FillGridWithChessPatern()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var rect = new Rectangle();
                    if ((row + col) % 2 == 0)
                    {
                        rect.Fill = Constants.WHITECASESCOLOR;
                    }
                    else
                    {
                        rect.Fill = Constants.BLACKCASESCOLOR;
                    }
                    rect.MouseDown += Rect_MouseDown;
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);
                    myGrid.Children.Add(rect);
                }
            }
        }
        private void Rect_MouseDown(object sender, MouseButtonEventArgs e)
        {

            int row = 0;
            int col = 0;
            var clickedSquare = sender as Rectangle;
            if (clickedSquare != null)
            {                  
                row = Grid.GetRow(clickedSquare);
                col = Grid.GetColumn(clickedSquare);
            }
            else
            {
                var clickedSquare2 = sender as Label;
                row = Grid.GetRow(clickedSquare2);
                col = Grid.GetColumn(clickedSquare2);
            }
            if (_board.getTurn() == _board.get(row, col).isWhite())
            {
                FillGridWithChessPatern();
                FillChessPieces(ref _board);
                _possible_cases = new List<Tuple<byte, byte>>();
                List <Tuple<byte, byte>> another_cases = new List<Tuple<byte, byte>>();
                _selected_case = new Tuple<byte, byte>((byte)row, (byte)col);
                _board.get(row, col).getPossibilities(_board, out _possible_cases);
                _board.setPossibilities(_possible_cases);
                if(_board.getCheckMode() == true)
                {
                    _possible_cases = _board.get(row, col).getUncheckPossibilities(ref _board, _selected_case);
                }
                foreach (Tuple<byte, byte> tuple in _possible_cases)
                {
                    var rectangle = new Rectangle();
                    rectangle.Fill = Constants.ACTIVECASE;
                    Grid.SetRow(rectangle, tuple.Item1);
                    Grid.SetColumn(rectangle, tuple.Item2);
                    rectangle.MouseDown += Rectangle_Sel_MouseDown;
                    myGrid.Children.Add(rectangle);
                    _is_selected = true;
                }
            }

        }
        private void Rectangle_Sel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = 0;
            int col = 0;
            var clickedSquare = sender as Rectangle;
            if (clickedSquare != null)
            {
                row = Grid.GetRow(clickedSquare);
                col = Grid.GetColumn(clickedSquare);
            }
            else
            {
                var clickedSquare2 = sender as Label;
                row = Grid.GetRow(clickedSquare2);
                col = Grid.GetColumn(clickedSquare2);
            }

            Tuple<byte, byte> select = new Tuple<byte, byte>((byte)row, (byte)col);
            Tuple<byte, byte> foundTuple = _possible_cases.Find(tuple => tuple.Item1 == select.Item1 && tuple.Item2 == select.Item2);
            if (foundTuple != null)
            {
                FillGridWithChessPatern();
                _board.move(_selected_case, select);
                FillChessPieces(ref _board);
                _is_selected = false;
            }
            else
            {
                FillGridWithChessPatern();
                FillChessPieces(ref _board);
                _is_selected = false;
            }

        }

        private void FillChessPieces(ref Board _board)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var board_case = _board.get(row, col);
                    typeOfPiece type = board_case.getType();
                    var color = new SolidColorBrush();
                    if (board_case.isWhite())
                    {
                        color = Constants.WHITEPIECESCOLOR;
                    }
                    else
                    {
                        color = Constants.BLACKPIECESCOLOR;
                    }
                    switch(type)
                    {
                        case typeOfPiece.Pawn:
                            var label = new Label
                            {
                                Content = "♟",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Foreground = color,
                                FontSize = 36
                            };
                            label.MouseDown += Rect_MouseDown;
                            Grid.SetRow(label, row);
                            Grid.SetColumn(label, col);
                            myGrid.Children.Add(label);
                            break;
                        case typeOfPiece.Rook:
                            label = new Label
                            {
                                Content = "♜",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Foreground = color,
                                FontSize = 36
                            };
                            label.MouseDown += Rect_MouseDown;
                            Grid.SetRow(label, row);
                            Grid.SetColumn(label, col);
                            myGrid.Children.Add(label);
                            break;
                        case typeOfPiece.Bishop:
                            label = new Label
                            {
                                Content = "♝",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Foreground = color,
                                FontSize = 36
                            };
                            label.MouseDown += Rect_MouseDown;
                            Grid.SetRow(label, row);
                            Grid.SetColumn(label, col);
                            myGrid.Children.Add(label);
                            break;
                        case typeOfPiece.Knight:
                            label = new Label
                            {
                                Content = "♞",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Foreground = color,
                                FontSize = 36
                            };
                            label.MouseDown += Rect_MouseDown;
                            Grid.SetRow(label, row);
                            Grid.SetColumn(label, col);
                            myGrid.Children.Add(label);
                            break;
                        case typeOfPiece.King:
                            label = new Label
                            {
                                Content = "♚",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Foreground = color,
                                FontSize = 36
                            };
                            label.MouseDown += Rect_MouseDown;
                            Grid.SetRow(label, row);
                            Grid.SetColumn(label, col);
                            myGrid.Children.Add(label);
                            break;
                        case typeOfPiece.Queen:
                            label = new Label
                            {
                                Content = "♛",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Foreground = color,
                                FontSize = 36
                            };
                            label.MouseDown += Rect_MouseDown;
                            Grid.SetRow(label, row);
                            Grid.SetColumn(label, col);
                            myGrid.Children.Add(label);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
