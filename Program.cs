internal class Program
{
    public static int[,] _matrix { get; private set; } = new int[10, 10];

    public static void Main(string[] args)
    {
        ShowMatrix();

        //4,5  & 9,1
        var origin = new MatrixElement { Position = new Position(4, 5) };
        var destiny = new MatrixElement { Position = new Position(9, 1) };
        var cursor = origin.ClonePosition();

        var xdestinationDirection = Direction.Undefine;
        var ydestinationDirection = Direction.Undefine;


        if (destiny.Position.X > origin.Position.X)
            xdestinationDirection = Direction.Rigth;

        if (destiny.Position.X < origin.Position.X)
            xdestinationDirection = Direction.Left;

        if (destiny.Position.Y < origin.Position.Y)
            ydestinationDirection = Direction.Up;

        if (destiny.Position.Y > origin.Position.Y)
            ydestinationDirection = Direction.Down;


        while (!CursorIsInFinalPosition(cursor, destiny))
        {
            int currentMatrixValue;
            currentMatrixValue = cursor.Value;

            //int nextStep = matrix[cursor[0], cursor[1]];
            Console.WriteLine($"\n\nEl destino esta {ydestinationDirection} to {xdestinationDirection}\n");


            int posibleYStep;
            int posibleXStep;
            var posibleStep = cursor.ClonePosition();

            if (destiny.Position.X > origin.Position.X)
            {
                xdestinationDirection = Direction.Rigth;
                posibleStep.MoveToRight();
                posibleXStep = posibleStep.Value;
            }

            //...

            // if (ydestinationDirection == Direction.Up)
            //     posibleYStep = matrix[StepUp(cursor).x, StepUp(cursor).y];

            // if (ydestinationDirection == Direction.Down)
            //     posibleYStep = matrix[StepDown(cursor).x, StepDown(cursor).y];

            // if (xdestinationDirection == Direction.Left)
            //     posibleXStep = matrix[StepLeft(cursor).x, StepLeft(cursor).y];

            // if (xdestinationDirection == Direction.Rigth)
            //     posibleXStep = matrix[StepRight(cursor).x, StepRight(cursor).y];



            return;
        }


    }

    static bool CursorIsInFinalPosition(MatrixElement cursor, MatrixElement destiny)
                => cursor.Position.X == destiny.Position.X &&
                   cursor.Position.Y == destiny.Position.Y;
    static void ShowMatrix()
    {
        var rand = new Random();
        Console.WriteLine("\t|0| \t|1|\t|2|\t|3|\t|4|\t|5|\t|6|\t|7|\t|8|\t|9|\n");
        for (int x = 0; x < 10; x++)
        {
            Console.Write($"{x} ==");
            for (int y = 0; y < 10; y++)
            {
                var matrixValue = rand.Next(1, 10);
                _matrix[x, y] = matrixValue;
                Console.Write($"\t {matrixValue}");
            }
            Console.WriteLine();
            Console.WriteLine("\t---------------------------------------------------------------------------");
        }
    }

    record class MatrixElement
    {
        public Position Position { get; set; }
        public int Value => _matrix[Position.X, Position.Y];


        public void MoveToUp() => Position.Y--;
        public void MoveToDown() => Position.Y++;
        public void MoveToLeft() => Position.X--;
        public void MoveToRight() => Position.X++;


        public MatrixElement ClonePosition()
        {
            return new MatrixElement
            {
                Position = new Position(this.Position.X, this.Position.Y)
            };
        }
    }

    static void Monito(MatrixElement origin, MatrixElement target)
    {
        var cursor = origin.ClonePosition();

        if (CursorIsInFinalPosition(cursor, target))
            return;


    }

    public record Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    };

    enum Direction
    {
        Undefine,
        Up,
        Down,
        Left,
        Rigth
    }
}

