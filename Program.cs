internal class Program
{
    public static int MatrixDimention = 5;
    public static int[,] _matrix { get; private set; } = new int[MatrixDimention, MatrixDimention];

    public static void Main(string[] args)
    {
        var initialTime = DateTime.Now;
        ShowMatrix();

        //4,5  & 9,1
        var origin = new MatrixElement { Position = new Position(0, 2) };
        var destiny = new MatrixElement { Position = new Position(3, 3) };
        var cursor = origin.CopyPosition();

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

        Console.WriteLine($"\n\nOrigen: ({origin.Position.X},{origin.Position.Y}) => {origin.Value} --- Destino: ({destiny.Position.X},{destiny.Position.Y}) => {destiny.Value}");
        Console.WriteLine($"El destino esta {ydestinationDirection} to {xdestinationDirection}\n");

        // return;
        while (!CursorIsInFinalPosition(cursor, destiny))
        {
            var xAxisPosibleStep = cursor.CopyPosition();
            var yAxisPosibleStep = cursor.CopyPosition();

            xAxisPosibleStep = GetXPosibleStep(xAxisPosibleStep, cursor, xdestinationDirection);
            yAxisPosibleStep = GetYPosibleStep(yAxisPosibleStep, cursor, ydestinationDirection);

            if (xAxisPosibleStep.Value < yAxisPosibleStep.Value)
                cursor = xAxisPosibleStep.CopyPosition();
            else
                cursor = yAxisPosibleStep.CopyPosition();

        }

        Console.WriteLine($"El valor del cursor es: {cursor.Value}. Posicion: ({cursor.Position.X}, {cursor.Position.Y})");
        var finalTime = DateTime.Now;
        var executionTime = finalTime - initialTime;
        Console.WriteLine($"Tiempo de ejecucion: {executionTime}");

        static MatrixElement GetXPosibleStep(MatrixElement posibleStep, MatrixElement cursor, Direction xdestinationDirection)
        {
            if (xdestinationDirection == Direction.Rigth)
            {
                if (cursor.Position.X == MatrixDimention - 1)
                {
                    xdestinationDirection = ChangeXDirection(xdestinationDirection);
                    GetXPosibleStep(posibleStep, cursor, xdestinationDirection);
                }
                else
                    posibleStep.MoveToRight();
            }
            if (xdestinationDirection == Direction.Left)
            {
                if (cursor.Position.X == 0)
                {
                    xdestinationDirection = ChangeXDirection(xdestinationDirection);
                    GetXPosibleStep(posibleStep, cursor, xdestinationDirection);
                }
                else
                    posibleStep.MoveToLeft();
            }

            return posibleStep;
        }

        static Direction ChangeXDirection(Direction xdestinationDirection)
            => xdestinationDirection == Direction.Rigth ? Direction.Left : Direction.Rigth;


        static MatrixElement GetYPosibleStep(MatrixElement posibleStep, MatrixElement cursor, Direction ydestinationDirection)
        {
            if (ydestinationDirection == Direction.Down)
            {
                if (cursor.Position.Y == MatrixDimention - 1)
                {
                    ydestinationDirection = ChangeYDirection(ydestinationDirection);
                    return GetYPosibleStep(posibleStep, cursor, ydestinationDirection);
                }
                else
                    posibleStep.MoveToDown();
            }
            if (ydestinationDirection == Direction.Up)
            {
                if (cursor.Position.Y == 0)
                {
                    ydestinationDirection = ChangeYDirection(ydestinationDirection);
                    return GetXPosibleStep(posibleStep, cursor, ydestinationDirection);
                }
                else
                    posibleStep.MoveToUp();
            }

            return posibleStep;
        }
        static Direction ChangeYDirection(Direction ydestinationDirection)
                    => ydestinationDirection == Direction.Up ? Direction.Down : Direction.Up;
    }

    static bool CursorIsInFinalPosition(MatrixElement cursor, MatrixElement destiny)
                => cursor.Position.X == destiny.Position.X &&
                   cursor.Position.Y == destiny.Position.Y;

    static void ShowMatrix()
    {
        var rand = new Random();

        for (int h = 0; h < MatrixDimention; h++)
            Console.Write($"\t|{h}|");

        Console.WriteLine("\n");

        for (int y = 0; y < MatrixDimention; y++)
        {
            Console.Write($"|{y}|");
            for (int x = 0; x < MatrixDimention; x++)
            {
                var matrixValue = rand.Next(1, 10);
                _matrix[x, y] = matrixValue;
                Console.Write($"\t {matrixValue}");
            }

            Console.WriteLine();
            Console.WriteLine("\t---------------------------------------");
        }
    }

    public record class MatrixElement
    {
        public Position Position { get; set; }
        public int Value => _matrix[Position.X, Position.Y];


        public void MoveToUp() => Position.Y--;
        public void MoveToDown() => Position.Y++;
        public void MoveToLeft() => Position.X--;
        public void MoveToRight() => Position.X++;


        public MatrixElement CopyPosition()
        {
            return new MatrixElement
            {
                Position = new Position(this.Position.X, this.Position.Y)
            };
        }
    }

    static void Monito(MatrixElement origin, MatrixElement target)
    {
        var cursor = origin.CopyPosition();

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


    public record class Vertice
    {
        public Vertice()
        {
            Aristas = new();
        }
        public int Value { get; set; }
        public static List<MatrixElement> Aristas { get; set; } = new List<MatrixElement>();

        public static void Create(int[,] matrix, MatrixElement element)
        {
            var vertice = new Vertice();
            vertice.Value = element.Value;

            if(element.Position.X > 0)
            {
                var newElement = element.CopyPosition();
                newElement.MoveToLeft();
                Aristas.Add(newElement);
            }
        }
    }
}

