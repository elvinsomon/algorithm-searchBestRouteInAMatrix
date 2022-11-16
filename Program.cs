using System.Reflection.Metadata.Ecma335;

internal class Program
{
    private static IDictionary<int, Position> _indexPositionDictionary = new Dictionary<int, Position>();
    const int INF = 10000;
    private static Graph _mainGraph = new();
    private const int _mainMatrixDimention = 3;
    private static Direction _xAxisTargetDirection;
    private static Direction _yAxisTargetDirection;
    private static Graph _bestRoute { get; set; } = new();
    private static readonly Dictionary<Direction, string> _directionTranslation = new();
    private static int[,] _mainMatrix { get; set; } = new int[_mainMatrixDimention, _mainMatrixDimention];
    private static bool _showRawWightMatrix = false;
    private static int[,] _rawWeightMatrix { get; set; } = new int[_mainMatrixDimention * _mainMatrixDimention, _mainMatrixDimention * _mainMatrixDimention];
    private static int[,] _rawPathMatrix { get; set; } = new int[_mainMatrixDimention * _mainMatrixDimention, _mainMatrixDimention * _mainMatrixDimention];

    public static void Main(string[] args)
    {
        var initialTime = DateTime.Now;
        var origin = new MatrixElement { Position = new Position(0, 1) }; //4,5  & 9,1
        var target = new MatrixElement { Position = new Position(2, 2) };

        ShowMatrix(origin, target);
        ShowPointsInformation(origin, target);
        GenerateGraph();


        GeneratePositionIndexEquivalation();

        _rawPathMatrix = CreatePathMatrix();
        _rawWeightMatrix = CreateRawWeightMatrix();

        var equivalentMatrix = FloydWarshall(_rawWeightMatrix);

        ShowPathMatrix("(Actual)");

        var originIndexInEquivalentMatrix = GetDictionaryKey(origin.Position);
        var targetIndexInEquivalentMatrix = GetDictionaryKey(target.Position);

        var shortestWeight = equivalentMatrix[originIndexInEquivalentMatrix, targetIndexInEquivalentMatrix];
        Console.WriteLine($"\n\nEl camino con menor peso del punto ({origin.Position.X},{origin.Position.Y}) a ({target.Position.X},{target.Position.Y}) es de: {shortestWeight}");



        FindBestRouteInPathMatrix(originIndexInEquivalentMatrix, targetIndexInEquivalentMatrix);

        //Revisar...
        // Console.WriteLine("\n\n\n------------------------------------------------------------------------------------");
        // Console.WriteLine("\n\n\nRecorrido: ");
        // Console.WriteLine($" · {origin.Value} (Origen)");
        // CalculateBestRoute(origin, target);
        // Console.WriteLine($" · {target.Value} (Destino)");

        // Console.WriteLine();
        // var totalGraphWeigh = _bestRoute.Nodes.Sum(x => x.Value);

        // var totalGraphSteps = _bestRoute.Nodes.Count - 1;
        // Console.WriteLine($"Peso:\t{totalGraphWeigh}");
        // Console.WriteLine($"Pasos:\t{totalGraphSteps}");

        var finalTime = DateTime.Now;
        var executionTime = finalTime - initialTime;
        Console.WriteLine($"\nTiempo de ejecucion: {executionTime}");
    }


    private static void FindBestRouteInPathMatrix(int originPoint, int targetPoint)
    {

        //Ruta
        var current = originPoint;
        while (current != targetPoint)
        {
            current = _rawPathMatrix[current, targetPoint];
            Console.WriteLine($" · {_indexPositionDictionary[current].X},{_indexPositionDictionary[current].Y}");
        }
    }

    private static int GetDictionaryKey(Position position)
    {
        foreach (var item in _indexPositionDictionary)
            if (item.Value.X == position.X && item.Value.Y == position.Y)
                return item.Key;

        return -1;
    }

    #region Generate Graph & Distance Matrix
    private static void GenerateGraph()
    {
        for (int y = 0; y < _mainMatrixDimention; y++)
        {
            for (int x = 0; x < _mainMatrixDimention; x++)
            {
                var newNode = new Node(_mainMatrix, new MatrixElement { Position = new Position(x, y) });
                _mainGraph.Nodes.Add(newNode);
            }
        }

        System.Console.WriteLine("Grafo generado a partir de los datos de la matriz");
        System.Console.WriteLine($"Cantidad de Nodos: \t{_mainGraph.Nodes.Count}");
        System.Console.WriteLine($"Cantidad de Aristas: \t{_mainGraph.EdgesCount}");
    }

    private static void GeneratePositionIndexEquivalation()
    {
        var index = 0;
        for (int y = 0; y < _mainMatrixDimention; y++)
            for (int x = 0; x < _mainMatrixDimention; x++)
                _indexPositionDictionary.Add(index++, new Position(x, y));

        var dictionaryKeys = _indexPositionDictionary.Keys.ToList();
        Console.WriteLine($"\n\n\nDICCIONARIO DE ÍNDICE POSICIÓN");
        Console.WriteLine($"------------------------------");
        Console.WriteLine($"\nIndice\t\tPosicion\tValor  ");
        Console.WriteLine($"--------------------------------------");
        foreach (var item in dictionaryKeys)
        {
            var position = _indexPositionDictionary[item];
            var value = _mainMatrix[position.X, position.Y];
            Console.WriteLine($"{item} \t\t{position.X},{position.Y} \t\t{value}");
        }
    }

    private static int[,] CreatePathMatrix()
    {
        for (int i = 0; i < _rawPathMatrix.GetLength(0); i++)
            for (int j = 0; j < _rawPathMatrix.GetLength(0); j++)
                _rawPathMatrix[i, j] = j;

        //ShowPathMatrix("(CRUDA)");
        return _rawPathMatrix;
    }


    private static void ShowPathMatrix(string tittle = "")
    {

        Console.WriteLine("\n\n\n");
        Console.WriteLine($"MATRIZ DE RECORRIDOS {tittle}");
        Console.WriteLine("----------------------------");
        Console.WriteLine("\n");

        for (int h = 0; h < _rawPathMatrix.GetLength(0); h++)
            Console.Write($"\t|{h}|");

        Console.WriteLine("\n");

        for (int i = 0; i < _rawPathMatrix.GetLength(0); i++)
        {
            Console.Write($"|{i}|");
            for (int j = 0; j < _rawPathMatrix.GetLength(0); j++)
                Console.Write($"\t {_rawPathMatrix[i, j]}");

            System.Console.WriteLine();
        }
    }

    private static int[,] CreateRawWeightMatrix()
    {
        var weightMatrixDimention = _mainGraph.Dimention;
        var rawWeightMatrix = new int[weightMatrixDimention, weightMatrixDimention];

        for (int y = 0; y < rawWeightMatrix.GetLength(0); y++)
        {
            var currentNodePosition = _indexPositionDictionary[y];
            var currentNode = _mainGraph.Nodes.FirstOrDefault(n => n.Position.X == currentNodePosition.X && n.Position.Y == currentNodePosition.Y);

            for (int x = 0; x < rawWeightMatrix.GetLength(0); x++)
            {
                if (x == y)
                {
                    rawWeightMatrix[x, y] = 0;
                }
                else
                {
                    var nodeEdgesInCurrentPosition = currentNode?.Edges.FirstOrDefault(e => e.Element.Position.X == _indexPositionDictionary[x].X && e.Element.Position.Y == _indexPositionDictionary[x].Y);
                    if (nodeEdgesInCurrentPosition is null)
                        rawWeightMatrix[y, x] = INF;
                    else
                        rawWeightMatrix[y, x] = _mainMatrix[nodeEdgesInCurrentPosition.Element.Position.X, nodeEdgesInCurrentPosition.Element.Position.Y];
                }
            }
        }

        if (_showRawWightMatrix)
            ShowRawWeightMatrix(rawWeightMatrix);

        return rawWeightMatrix;
    }

    static void ShowRawWeightMatrix(int[,] rawWeightMatrix)
    {
        Console.WriteLine("\n\n\n");
        Console.WriteLine("MATRIZ DE PESOS (CRUDA)");
        Console.WriteLine("-----------------------");
        Console.WriteLine("\n");


        for (int h = 0; h < rawWeightMatrix.GetLength(0); h++)
            Console.Write($"\t|{h}|");

        Console.WriteLine("\n");

        for (int y = 0; y < rawWeightMatrix.GetLength(0); y++)
        {
            Console.Write($"|{y}|");
            for (int x = 0; x < rawWeightMatrix.GetLength(0); x++)
            {
                var matrixValue = rawWeightMatrix[y, x] == INF ? "¿" : rawWeightMatrix[y, x].ToString();

                Console.Write($"\t {matrixValue}");
            }

            Console.Write("\n\t");
            for (int i = 0; i < _mainMatrixDimention * 39; i++)
                Console.Write("-");

            Console.WriteLine();
        }
    }
    #endregion


    public static int[,] FloydWarshall(int[,] rawWeightMatrix)
    {
        var equivalentMatrix = rawWeightMatrix;
        for (int k = 0; k < equivalentMatrix.GetLength(0); k++)
        {
            for (int i = 0; i < equivalentMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < equivalentMatrix.GetLength(0); j++)
                {
                    if (equivalentMatrix[i, j] > equivalentMatrix[i, k] + equivalentMatrix[k, j])
                    {
                        equivalentMatrix[i, j] = equivalentMatrix[i, k] + equivalentMatrix[k, j];
                        _rawPathMatrix[i, j] = k;
                    }
                }
            }
        }

        DisplayShortDistantMatrix(equivalentMatrix);
        return equivalentMatrix;
    }

    private static void DisplayShortDistantMatrix(int[,] equivalentMatrix)
    {
        Console.WriteLine("\n\n\n");
        Console.WriteLine("Aplicar algoritmo de Floyd\n");
        Console.WriteLine("MATRIZ DE DISTANCIAS DE FLOYD-WARSHALL");
        Console.WriteLine("--------------------------------------\n");

        for (int h = 0; h < equivalentMatrix.GetLength(0); h++)
            Console.Write($"\t|{h}|");

        Console.WriteLine("\n");

        for (int i = 0; i < equivalentMatrix.GetLength(0); ++i)
        {
            Console.Write($"|{i}|");
            for (int j = 0; j < equivalentMatrix.GetLength(0); ++j)
            {
                if (equivalentMatrix[i, j] == INF)
                    Console.Write("\tINF");
                else
                    Console.Write($"\t{equivalentMatrix[i, j]}");
            }

            Console.Write("\n\t");
            for (int l = 0; l < _mainMatrixDimention * 39; l++)
                Console.Write("-");

            Console.WriteLine();
        }
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
        _xAxisTargetDirection = GetXAxisTargetDirection(origin, destiny);
        _yAxisTargetDirection = GetYAxisTargetDirection(origin, destiny);
        Console.WriteLine($"\nEl destino se encuentra {_directionTranslation[_yAxisTargetDirection]} a la {_directionTranslation[_xAxisTargetDirection]} del origen.\n");
    }

    private static void LoadDirectionTranslation()
    {
        _directionTranslation.Add(Direction.Top, "Arriba");
        _directionTranslation.Add(Direction.Bottom, "Abajo");
        _directionTranslation.Add(Direction.Left, "Izquierda");
        _directionTranslation.Add(Direction.Right, "Derecha");
    }

    static void ShowMatrix(MatrixElement origin, MatrixElement target)
    {
        var rand = new Random();
        Console.WriteLine("\n");
        Console.WriteLine("Matriz Original");
        Console.WriteLine("---------------");
        Console.WriteLine("\n");

        for (int h = 0; h < _mainMatrixDimention; h++)
            Console.Write($"\t|{h}|");

        Console.WriteLine("\n");

        for (int y = 0; y < _mainMatrixDimention; y++)
        {
            Console.Write($"|{y}|");
            for (int x = 0; x < _mainMatrixDimention; x++)
            {
                var matrixValue = rand.Next(1, 20);
                _mainMatrix[x, y] = matrixValue;

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
            for (int i = 0; i < _mainMatrixDimention * 7.8; i++)
                Console.Write("-");

            Console.WriteLine();
        }
    }

    private static readonly List<MatrixElement> TraveledElements = new();

    private static void CalculateBestRoute(MatrixElement origin, MatrixElement target)
    {
        var cursor = new Node(_mainMatrix, origin);
        _bestRoute.Nodes.Add(cursor);
        var cursorMatrixElement = new MatrixElement { Position = new Position(cursor.Position.X, cursor.Position.Y) };

        //Si el cursor esta en el destino, terminar el recorrido
        if (CursorIsInTargetPosition(cursorMatrixElement, target))
            return;

        //Evaluar si el target esta en la misma fila o columna que el cursor
        //De ser asi, se debe mover en linea recta hacia el target
        foreach (var cursorEdge in cursor.Edges.Where(cursorEdge => CursorIsInTargetPosition(cursorEdge.Element, target)))
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
        _xAxisTargetDirection = GetXAxisTargetDirection(cursorMatrixElement, target);
        _yAxisTargetDirection = GetYAxisTargetDirection(cursorMatrixElement, target);
        var edgesInTargetDirection = cursor.Edges.Where(x => x.Direction == _xAxisTargetDirection || x.Direction == _yAxisTargetDirection);
        var minorValueEdge = edgesInTargetDirection.OrderBy(x => x.Element.Value).First();

        Console.Write($" · {minorValueEdge.Element.Value} ");

        switch (minorValueEdge.Direction)
        {
            case Direction.Top:
                Console.Write("↑");
                break;
            case Direction.Bottom:
                Console.Write("↓");
                break;
            case Direction.Left:
                Console.Write("←");
                break;
            case Direction.Right:
                Console.Write("→");
                break;

            case Direction.Undefine:
            default:
                break;
        }

        Console.WriteLine();

        //A la funcion se le debe pasar el nodo actual, para que en la proxima iteracion sea descartado y no se vualva a procesar.
        //En la primera iteracion el nodo previo es el origen 
        //previousNode = previousNode is null ? origin : cursorMatrixElement;
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
            xDirection = Direction.Top;

        if (target.Position.Y > origin.Position.Y)
            xDirection = Direction.Bottom;

        return xDirection;
    }

    public record class MatrixElement
    {
        public Position Position { get; set; } = new(0, 0);
        public int Value => _mainMatrix[Position.X, Position.Y];
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
        Top,
        Bottom,
        Left,
        Right
    }

    public class Graph
    {
        public List<Node> Nodes { get; set; } = new();
        public int Dimention => Nodes.Count;
        public int EdgesCount => Nodes.ToList().Sum(x => x.Edges.Count);
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

            if (element.Position.X < _mainMatrixDimention - 1)
            {
                var newElement = element.CopyPosition();
                newElement.MoveToRight();
                Edges.Add(new Edge(newElement, Direction.Right));
            }

            if (element.Position.Y > 0)
            {
                var newElement = element.CopyPosition();
                newElement.MoveToUp();
                Edges.Add(new Edge(newElement, Direction.Top));
            }

            if (element.Position.Y < _mainMatrixDimention - 1)
            {
                var newElement = element.CopyPosition();
                newElement.MoveToDown();
                Edges.Add(new Edge(newElement, Direction.Bottom));
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