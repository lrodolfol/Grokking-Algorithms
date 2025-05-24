using System.Diagnostics;
using ToweOfHanoi;

public class Game
{
    private readonly List<Tower> _towers = new List<Tower>(3);
    private Stack<int> _disksOrder = new Stack<int>();

    public Game(int numberOfDisks)
    {
        _towers.Add(new Tower(0, "Tower 1"));
        _towers.Add(new Tower(0, "Tower 2"));
        _towers.Add(new Tower(0, "Tower 3"));
        
        var cont = 0;
        for (int i = numberOfDisks; i > 0; i--)
        {
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
            AlterPopIfIsBigger(ref randomTowerPush, ref randomTowerPop);
            AlterIfDiskIntoTowerIsSmallerThanPop(randomTowerPop, ref randomTowerPush);


            AlterPopAndPushIfOneDiskIsMissingForLastTower(ref randomTowerPop, ref randomTowerPush);

            if (!_towers[randomTowerPop].IsEmpty())
            {
                if (!_towers[randomTowerPush].IsEmpty())
                {
                    if (_towers[randomTowerPush].Disks.Peek() < _towers[randomTowerPop].Disks.Peek())
                    {
                        Console.WriteLine($"opaa");
                        Console.ReadKey();
                    }
                }
            }
            
            //os dois primeiros movimentos sempre devem retirar discos da torre 0 e colocar numa torre vazia
            if (attempts < 2)
                AlterPopForFirstTower(ref randomTowerPop, ref randomTowerPush);
            
            //na terceira jogada, o menor disco sempre irá para a torre com o 2ª menor disco, em outras palavras, o disco nunca vai para torre 0
            if(attempts == 2)
                AlterPopAndPushIfIsThirdMovement(ref randomTowerPop, ref randomTowerPush);
             
            //no quarto movimento, o pop deve ser na primeira torre para uma torre vazia
            if (attempts == 3)
                AlterPopAndPushIfFourth(ref randomTowerPop, ref randomTowerPush);
            
            

            //o menor disco nunca pode fica como primeiro disco de uma das torres, exeto se um das torrers conter todos os discos
            AlterPopAndPushIfFistDiskFromPushIsSmaller(ref randomTowerPop, ref randomTowerPush);

            //verificar se a torre 3 esta vazia e se há uma torre com um unico disco que seja do maior tamanho. Colocar esse diso maior na torre 3
            MoveBiggerDiskToTower3IfIsEmpty(ref randomTowerPop, ref randomTowerPush);

            //se o menor disco esta no topo de alguma torre, esse disco é o que precisa ser movido (pop)
            //AlterPopAndPushIfSmallerDiskOnTheTop(ref randomTowerPop, ref randomTowerPush);

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

    private void AlterPopAndPushIfFourth(ref int randomTowerPop, ref int randomTowerPush)
    {
        var tower = _towers.Where(x => x.IsEmpty()).Single();
        
        randomTowerPush = _towers.IndexOf(tower);
        randomTowerPop = 0;
    }

    private void AlterPopAndPushIfIsThirdMovement(ref int randomTowerPop, ref int randomTowerPush)
    {
        var options = new List<int> { 0, 1, 2 };
        options.Remove(0);

        var tower = _towers.First(x => x.Disks.Count > 0 && x.Disks.Peek() == (_disksOrder.First() * 10));
        options.Remove(_towers.IndexOf(tower));
        
        randomTowerPop = _towers.IndexOf(tower);
        randomTowerPush = options.First();
    }

    private void AlterPopForFirstTower(ref int randomTowerPop, ref int randomTowerPush)
    {
        randomTowerPop = 0;
        randomTowerPush = _towers[1].IsEmpty() ? 1 : 2;
    }

    //Nao é possivel mover o disco maior para uma torre que já tem disco
    private void AlterPopIfIsBigger(ref int randomTowerPush, ref int randomTowerPop)
    {
        if (_towers[randomTowerPush].IsEmpty())
            return;

        var pushBkp = randomTowerPush;
        var popBkp = randomTowerPop;

        if (_towers[randomTowerPop].Disks.Peek() != (_disksOrder.Last() * 10))
            return;

        randomTowerPop = pushBkp;
        randomTowerPush = popBkp;
    }

    private void AlterPopAndPushIfFistDiskFromPushIsSmaller(ref int randomTowerPop, ref int randomTowerPush)
    {
        if (_towers[randomTowerPop].IsFull())
            return;

        if (_towers[randomTowerPush].IsEmpty())
            return;

        if (_towers[randomTowerPop].HasDisks() && _towers[randomTowerPop].Disks.Peek() == _disksOrder.First())
        {
            var options = new List<int> { 0, 1, 2 };
            options.Remove(randomTowerPop);

            //verificar qual torre esta vazia
            if (_towers[options.First()].HasDisks())
                randomTowerPush = options.First();
            else
                randomTowerPush = options.Last();
        }
    }

    private void MoveBiggerDiskToTower3IfIsEmpty(ref int randomTowerPop, ref int randomTowerPush)
    {
        if (! _towers[2].IsEmpty())
            return;
        
        //Busca a possivel torre cujo o disco do topo seja o maior disco
        var tower = _towers.FirstOrDefault(x => x.Disks.Count > 0 && x.Disks.Peek() == (_disksOrder.Last() * 10));

        if (tower is null)
            return;

        var options = new List<int> { 0, 1, 2 };
        
        randomTowerPush = 2;
        options.Remove(2);
        
        randomTowerPop = _towers.IndexOf(tower);
    }

    private void AlterPopAndPushIfOneDiskIsMissingForLastTower(ref int randomTowerPop, ref int randomTowerPush)
    {
        if (_towers[2].Disks.Count + 1 < _disksOrder.Count)
            return;

        //se for tirar do 2 ou o disco de onde for tirar for 2 não é o menor disco, entao sai
        if ((randomTowerPop == 2) || (_towers[randomTowerPop].Disks.Peek() != (_disksOrder.First() * 10)))
            return;

        var options = new List<int> { 0, 1, 2 };

        randomTowerPush = 2;
        options.Remove(2);

        if (_towers[options.First()].Disks.Count > 0)
            randomTowerPop = options.First();
        else
            randomTowerPop = options.Last();
    }

    private void AlterIfDiskIntoTowerIsSmallerThanPop(int randomTowerPop, ref int randomTowerPush)
    {
        //Console.WriteLine(nameof(AlterIfDiskIntoTowerIsSmallerThanPop));

        if (_towers[randomTowerPop].Disks.Count == 0)
            return;

        if (_towers[randomTowerPush].IfLastDiskSmaller(_towers[randomTowerPop].Disks.Peek()) == false)
            return;

        var options = new List<int> { 0, 1, 2 };

        //Se o primeiro disco da pilha for menor que o disco que está sendo inserido, não pode colocar la
        //Nem colocar de onde estiver tirando o disco

        options.Remove(randomTowerPush);
        options.Remove(randomTowerPop);

        randomTowerPush = options.First();
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
            options.Remove(randomTowerPop);
            options.Remove(randomTowerPush);
        }
        else
        {
            options.Remove(randomTowerPush);
            options.Remove(randomTowerPop);
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
        options.Remove(randomTowerPush);
        options.Remove(randomTowerPop);
        
        randomTowerPop = options.First();
    }

    private void AlterPopIfBothIsEmpty(ref int randomTowerPop, int randomTowerPush)
    {
        //Console.WriteLine(nameof(AlterPopIfBothIsEmpty));

        if (!(_towers[randomTowerPop].Disks.Count == 0 || _towers[randomTowerPush].Disks.Count == 0))
            return;

        var options = new List<int> { 0, 1, 2 };

        //Se as duas torres estiverem vazias, não pode tirar de lá
        options.Remove(randomTowerPush);
        options.Remove(randomTowerPop);

        randomTowerPop = options.First();
    }

    private static void AlterIfPopAndPushIsEquals(ref int randomTowerPop, int randomTowerPush)
    {
        //Console.WriteLine(nameof(AlterIfPopAndPushIsEquals));

        if (randomTowerPop != randomTowerPush)
            return;

        var options = new List<int> { 0, 1, 2 };

        options.Remove(randomTowerPop);
        randomTowerPop = new Random().Next(0, 11) % 2 == 0 ? options.First() : options.Last();
    }

    private void Win(int attemps, Stopwatch stopwatch)
    {
        Console.WriteLine("Finalizado!");
        Console.WriteLine($"Número de tentativas: {attemps}");
        Console.WriteLine($"Tempo gasto: {stopwatch.Elapsed}");
    }
}