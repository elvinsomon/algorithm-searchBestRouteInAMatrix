internal class Program
{
    public static int MatrixDimention = 10;
    public static int[,] _matrix { get; private set; } = new int[MatrixDimention, MatrixDimention];
    public static Direction xAxisTargetDirection;
    public static Direction yAxisTargetDirection;
    public static Dictionary<Direction, string> DirectionTranslation = new();
    public static Graph BestRoute { get; set; } = new();

    public static void Main(string[] args)
    {
        var initialTime = DateTime.Now;
        var origin = new MatrixElement { Position = new Position(4, 5) }; //4,5  & 9,1
        var target = new MatrixElement { Position = new Position(9, 1) };

        ShowMatrix(origin, target);
        ShowPointsInformation(origin, target);

        //Revisar...
        Console.WriteLine("\nRecorrido: ");
        Console.WriteLine($" · {origin.Value} (Origen)");
        CalculateBestRoute(origin, target);
        Console.WriteLine($" · {target.Value} (Destino)");

        Console.WriteLine();
        var totalGraphWeigh = BestRoute.Nodes.Sum(x => x.Value);
        var totalGraphSteps = BestRoute.Nodes.Count - 1;
        Console.WriteLine($"Peso:\t{totalGraphWeigh}");
        Console.WriteLine($"Pasos:\t{totalGraphSteps}");

        var finalTime = DateTime.Now;
        var executionTime = finalTime - initialTime;
        Console.WriteLine($"\nTiempo de ejecucion: {executionTime}");
    }

    private static void ShowPointsInformation(MatrixElement origin, MatrixElement destiny)
    {
        Console.WriteLine();
        Console.Write("Origen: ");
        Console.BackgroundColor = ConsoleColor.DarkCyan;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" ({origin.Position.X},{origin.Position.Y}) => {origin.Value} ");
        Console.ResetColor();

        Console.Write(" --- ");

        Console.Write("Destino: ");
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" ({destiny.Position.X},{destiny.Position.Y}) => {destiny.Value} ");
        Console.ResetColor();


        LoadDirectionTranslation();
        xAxisTargetDirection = GetXAxisTargetDirection(origin, destiny);
        yAxisTargetDirection = GetYAxisTargetDirection(origin, destiny);
        Console.WriteLine($"\nEl destino se encuentra {DirectionTranslation[yAxisTargetDirection]} a la {DirectionTranslation[xAxisTargetDirection]} del origen.\n");
    }

    private static void LoadDirectionTranslation()
    {
        DirectionTranslation.Add(Direction.Up, "Arriba");
        DirectionTranslation.Add(Direction.Down, "Abajo");
        DirectionTranslation.Add(Direction.Left, "Izquierda");
        DirectionTranslation.Add(Direction.Right, "Derecha");
    }

    static void ShowMatrix(MatrixElement origin, MatrixElement target)
    {
        var rand = new Random();
        Console.WriteLine("\n\n");

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

                if (origin.Position.X == x && origin.Position.Y == y)
                {
                    Console.Write($"\t");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($" {matrixValue} ");
                    Console.ResetColor();
                }
                else if (target.Position.X == x && target.Position.Y == y)
                {
                    Console.Write($"\t");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($" {matrixValue} ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write($"\t {matrixValue}");
                    Console.ResetColor();
                }
            }

            Console.Write("\n\t");
            for (int i = 0; i < MatrixDimention * 7.8; i++)
                Console.Write("-");

            Console.WriteLine();
        }
    }

    private static List<MatrixElement> TraveledElements = new();
    static void CalculateBestRoute(MatrixElement origin, MatrixElement target)
    {
        var cursor = new Node(_matrix, origin);
        BestRoute.Nodes.Add(cursor);
        var cursorMatrixElement = new MatrixElement { Position = new Position(cursor.Position.X, cursor.Position.Y) };

        //Si el cursor esta en el destino, terminar el recorrido
        if (CursorIsInTargetPosition(cursorMatrixElement, target))
            return;

        //Evaluar si el target esta en la misma fila o columna que el cursor
        //De ser asi, se debe mover en linea recta hacia el target
        foreach (var cursorEdge in cursor.Edges)
            if (CursorIsInTargetPosition(cursorEdge.Element, target))
            {
                CalculateBestRoute(cursorEdge.Element, target);
                return;
            }

        //Si el cursor apunta a una de las aristas previamente recorridas, eliminarla de la lista de aristas
        foreach (var elementsToDelete in TraveledElements)
            cursor.Edges.RemoveAll(x => x.Element == elementsToDelete);

        //Agregar a la lista de elementos recorridos el elemento actual
        TraveledElements.Add(cursorMatrixElement);

        //Tomar las aristas que este en la direccion del target
        xAxisTargetDirection = GetXAxisTargetDirection(cursorMatrixElement, target);
        yAxisTargetDirection = GetYAxisTargetDirection(cursorMatrixElement, target);
        var edgesInTargetDirection = cursor.Edges.Where(x => x.Direction == xAxisTargetDirection || x.Direction == yAxisTargetDirection);
        var minorValueEdge = edgesInTargetDirection.OrderBy(x => x.Element.Value).First();

        Console.Write($" · {minorValueEdge.Element.Value} ");

        switch (minorValueEdge.Direction)
        {
            case Direction.Up:
                Console.Write("↑");
                break;
            case Direction.Down:
                Console.Write("↓");
                break;
            case Direction.Left:
                Console.Write("←");
                break;
            case Direction.Right:
                Console.Write("→");
                break;

            default:
                break;
        }
        
        Console.WriteLine();
        
        //A la funcion se le debe pasar el nodo actual, para que en la proxima iteracion sea descartado y no se vualva a procesar.
        //En la primera iteracion el nodo previo es el origen 
        //previuosNode = previuosNode is null ? origin : cursorMatrixElement;
        CalculateBestRoute(minorValueEdge.Element, target);
    }

    static bool CursorIsInTargetPosition(MatrixElement cursor, MatrixElement destiny)
                => cursor.Position.X == destiny.Position.X &&
                   cursor.Position.Y == destiny.Position.Y;

    private static Direction GetXAxisTargetDirection(MatrixElement origin, MatrixElement target)
    {
        var xDirection = Direction.Undefine;
        if (target.Position.X > origin.Position.X)
            xDirection = Direction.Right;

        if (target.Position.X < origin.Position.X)
            xDirection = Direction.Left;

        return xDirection;
    }

    private static Direction GetYAxisTargetDirection(MatrixElement origin, MatrixElement target)
    {
        var xDirection = Direction.Undefine;

        if (target.Position.Y < origin.Position.Y)
            xDirection = Direction.Up;

        if (target.Position.Y > origin.Position.Y)
            xDirection = Direction.Down;

        return xDirection;
    }

    public record class MatrixElement
    {
        public Position Position { get; set; } = new(0, 0);
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

    public enum Direction
    {
        Undefine,
        Up,
        Down,
        Left,
        Right
    }

    public class Graph
    {
        public List<Node> Nodes { get; set; } = new();
    }

    public class Node
    {
        public Node(int[,] matrix, MatrixElement element)
        {
            Value = element.Value;
            Position = new Position(element.Position.X, element.Position.Y);

            if (element.Position.X > 0)
            {
                var newElement = element.CopyPosition();
                newElement.MoveToLeft();
                Edges.Add(new Edge(newElement, Direction.Left));
            }

            if (element.Position.X < MatrixDimention - 1)
            {
                var newElement = element.CopyPosition();
                newElement.MoveToRight();
                Edges.Add(new Edge(newElement, Direction.Right));
            }

            if (element.Position.Y > 0)
            {
                var newElement = element.CopyPosition();
                newElement.MoveToUp();
                Edges.Add(new Edge(newElement, Direction.Up));
            }

            if (element.Position.Y < MatrixDimention - 1)
            {
                var newElement = element.CopyPosition();
                newElement.MoveToDown();
                Edges.Add(new Edge(newElement, Direction.Down));
            }
        }

        public int Value { get; set; }
        public Position Position { get; set; }
        public List<Edge> Edges { get; set; } = new List<Edge>();
    }

    public class Edge
    {
        public Edge(MatrixElement element, Direction direction)
        {
            Element = element;
            Direction = direction;
        }

        public MatrixElement Element { get; set; }
        public Direction Direction { get; set; }
    }
}