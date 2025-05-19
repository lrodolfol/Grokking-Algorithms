using System.Diagnostics;
using ToweOfHanoi;


Console.WriteLine("Jogo das Torres de Hanói");
Console.WriteLine("Aperte qualquer tecla para iniciar...");
//Console.ReadKey();

int disks = 0;
do
{
    Console.Clear();
    Console.WriteLine("Quantas torres você quer jogar? (3 a 8)");
    //disks = Convert.ToInt16(Console.ReadLine());
    disks = 3;
} while (disks < 3 || disks > 8);

new Game(disks).Run();

public class Game
{
    private readonly List<Tower> _towers = new List<Tower>(3);
    private Stack<int> _disksOrder = new Stack<int>();

    public Game(int numberOfDisks)
    {
        var cont = 0;
        for (int i = numberOfDisks; i > 0; i--)
        {
            _towers.Add(new Tower(numberOfDisks, $"Tower {cont + 1}"));
            _towers[0].Add((i) * 10);

            _disksOrder.Push(i);

            cont++;
        }
    }

    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();

        int attempts = 0;
        int randomTowerPop;
        int randomTowerPush;
    
        while (true)
        {
            foreach (var tower in _towers)
            {
                Console.WriteLine($"\n==={tower.TowerName}===");
                foreach (var disk in tower.Disks)
                    Console.WriteLine($"Disco: {disk}");
            }

            if (attempts > 0 && _towers[2].Verify())
            {
                stopwatch.Stop();

                Win(attempts, stopwatch);
                break;
            }

            randomTowerPop = new Random().Next(0, 3);
            randomTowerPush = new Random().Next(0, 3);

            AlterIfPopAndPushIsEquals(ref randomTowerPop, randomTowerPush);
            AlterPopIfBothIsEmpty(ref randomTowerPop, randomTowerPush);
            AlterPopIfEmpty(ref randomTowerPop, randomTowerPush);
            AlterPushIfFulled(ref randomTowerPush, randomTowerPop);
            AlterIfDiskIntoTowerIsSmallerThanPop(ref randomTowerPop, randomTowerPush);
            //AlterPopAndPushIfOneDiskIsMissingForLastTower(ref randomTowerPop, ref randomTowerPush);
            
            //o menor disco nunca pode fica como primeiro disco de uma das torres, exeto se um das torrers conter todos os discos
            //AlterPopAndPushIfFistDiskIsSmaller(ref randomTowerPop, ref randomTowerPush);
            
            //verificar se a torre 3 esta vazia e se há uma torre com um unico disco que seja do maior tamanho. Colocar esse diso maior na torre 3
            //MoveBiggerDiskToTower3IfIsEmpty(ref randomTowerPop, ref randomTowerPush);

            //se o menor disco esta no topo de alguma torre, esse disco é o que precisa ser movido (pop)
            ////AlterPopAndPushIfSmallerDiskOnTheTop(ref randomTowerPop, ref randomTowerPush);

            //fazer essa tarefa dentro do objeto
            var pop = _towers[randomTowerPop].Remove();
            if (pop > 0)
            {
                Console.WriteLine(
                    $"Removendo o disco {pop} da torre {_towers[randomTowerPop].TowerName} e Inserindo na torre {_towers[randomTowerPush].TowerName}");
                _towers[randomTowerPush].Add(pop);
            }

            attempts++;
        }
    }

    private void AlterPopAndPushIfFistDiskIsSmaller(ref int randomTowerPop, ref int randomTowerPush)
    {
        if (_towers[randomTowerPop].IsFull())
            return;

        if (_towers[randomTowerPush].HasDisks())
            return;

        if (_towers[randomTowerPop].HasDisks() && _towers[randomTowerPop].Disks.Peek() == _disksOrder.First())
        {
            var options = new List<int> { 0, 1, 2 };
            options.RemoveAt(randomTowerPop);

            //verificar qual torre esta vazia
            if (_towers[options.First()].HasDisks())
                randomTowerPush = options.First();
            else
                randomTowerPush = options.Last();
        }
    }

    private void MoveBiggerDiskToTower3IfIsEmpty(ref int randomTowerPop, ref int randomTowerPush)
    {
        if (_towers[2].IsEmpty() == false)
            return;

        var options = new List<int> { 0, 1, 2 };
        randomTowerPush = 2;
        options.RemoveAt(2);

        //Busca a possivel torre cujo o disco do topo seja o maior disco
        var tower = _towers.FirstOrDefault(x => x.Disks.Count > 0 && x.Disks.Peek() == _disksOrder.Last());

        if (tower is null)
            return;

        randomTowerPop = _towers.IndexOf(tower);
    }

    private void AlterPopAndPushIfOneDiskIsMissingForLastTower(ref int randomTowerPop, ref int randomTowerPush)
    {
        var options = new List<int> { 0, 1, 2 };

        if (_towers[2].Disks.Count + 1 < _disksOrder.Count)
            return;

        randomTowerPush = 2;
        options.RemoveAt(2);

        if (_towers[options.First()].Disks.Count > 0)
            randomTowerPop = options.First();
        else
            randomTowerPop = options.Last();
    }

    private void AlterIfDiskIntoTowerIsSmallerThanPop(ref int randomTowerPop, int randomTowerPush)
    {
        //Console.WriteLine(nameof(AlterIfDiskIntoTowerIsSmallerThanPop));

        if (_towers[randomTowerPop].Disks.Count == 0)
            return;

        if (_towers[randomTowerPush].IfLastDiskSmaller(_towers[randomTowerPop].Disks.Peek()) == false)
            return;

        var options = new List<int> { 0, 1, 2 };

        //Se o primeiro disco da pilha for menor que o disco que está sendo inserido, não pode colocar la
        //Nem colocar de onde estiver tirando o disco
        if (randomTowerPop > randomTowerPush)
        {
            options.RemoveAt(randomTowerPop);
            options.RemoveAt(randomTowerPush);
        }
        else
        {
            options.RemoveAt(randomTowerPush);
            options.RemoveAt(randomTowerPop);
        }

        randomTowerPop = options.First();
    }

    private void AlterPushIfFulled(ref int randomTowerPush, int randomTowerPop)
    {
        //Console.WriteLine(nameof(AlterPushIfFulled));

        if (_towers[randomTowerPush].IsFull() == false)
            return;

        var options = new List<int> { 0, 1, 2 };

        //Se a torre de onde está colocando o disco estiver cheia, não pode colocar la
        //Nem colocar de onde estiver tirando o disco
        if (randomTowerPop > randomTowerPush)
        {
            options.RemoveAt(randomTowerPop);
            options.RemoveAt(randomTowerPush);
        }
        else
        {
            options.RemoveAt(randomTowerPush);
            options.RemoveAt(randomTowerPop);
        }

        randomTowerPush = options.First();
    }

    private void AlterPopIfEmpty(ref int randomTowerPop, int randomTowerPush)
    {
        //Console.WriteLine(nameof(AlterPopIfEmpty));

        if (_towers[randomTowerPop].Disks.Count > 0)
            return;

        var options = new List<int> { 0, 1, 2 };

        //Nem tirar da que esta recebendo o disco
        //Se a torre de onde está tirando o disco estiver vazia, não pode tirar de lá
        if (randomTowerPop > randomTowerPush)
        {
            options.RemoveAt(randomTowerPop);
            options.RemoveAt(randomTowerPush);
        }
        else
        {
            options.RemoveAt(randomTowerPush);
            options.RemoveAt(randomTowerPop);
        }

        randomTowerPop = options.First();
    }

    private void AlterPopIfBothIsEmpty(ref int randomTowerPop, int randomTowerPush)
    {
        //Console.WriteLine(nameof(AlterPopIfBothIsEmpty));

        if (!(_towers[randomTowerPop].Disks.Count == 0 || _towers[randomTowerPush].Disks.Count == 0))
            return;

        var options = new List<int> { 0, 1, 2 };

        //Se as duas torres estiverem vazias, não pode tirar de lá
        if (randomTowerPop > randomTowerPush)
        {
            options.RemoveAt(randomTowerPop);
            options.RemoveAt(randomTowerPush);
        }
        else
        {
            options.RemoveAt(randomTowerPush);
            options.RemoveAt(randomTowerPop);
        }

        randomTowerPop = options.First();
    }

    private static void AlterIfPopAndPushIsEquals(ref int randomTowerPop, int randomTowerPush)
    {
        //Console.WriteLine(nameof(AlterIfPopAndPushIsEquals));

        if (randomTowerPop != randomTowerPush)
            return;

        var options = new List<int> { 0, 1, 2 };

        options.RemoveAt(randomTowerPop);
        randomTowerPop = new Random().Next(0, 11) % 2 == 0 ? options.First() : options.Last();
    }

    private void Win(int attemps, Stopwatch stopwatch)
    {
        Console.WriteLine("Finalizado!");
        Console.WriteLine($"Número de tentativas: {attemps}");
        Console.WriteLine($"Tempo gasto: {stopwatch.Elapsed}");
    }
}