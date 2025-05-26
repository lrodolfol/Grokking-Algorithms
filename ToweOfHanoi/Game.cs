using System.Diagnostics;

namespace ToweOfHanoi;

public class Game
{
    private readonly List<Tower> _towers = new List<Tower>(3);
    private Stack<int> _disksOrder = new Stack<int>();

    public int Attemps { get; private set; }
    public Stopwatch Stopwatch { get; private set; } = Stopwatch.StartNew();

    public Game(int numberOfDisks)
    {
        _towers.Add(new Tower(numberOfDisks, "Tower 1"));
        _towers.Add(new Tower(numberOfDisks, "Tower 2"));
        _towers.Add(new Tower(numberOfDisks, "Tower 3"));

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

            if (Attemps > 0 && _towers[2].Verify())
            {
                Stopwatch.Stop();

                Win(Attemps, Stopwatch);
                break;
            }

            randomTowerPop = new Random().Next(0, 3);
            randomTowerPush = new Random().Next(0, 3);

            if (Attemps <= 3)
            {
                //os dois primeiros movimentos sempre devem retirar discos da torre 0 e colocar numa torre vazia
                if (Attemps < 2)
                    AlterPopForFirstTower(ref randomTowerPop, ref randomTowerPush);

                //na terceira jogada, o menor disco sempre irá para a torre com o 2ª menor disco, em outras palavras, o disco nunca vai para torre 0
                if (Attemps == 2)
                    AlterPopAndPushIfIsThirdMovement(ref randomTowerPop, ref randomTowerPush);

                //no quarto movimento, o pop deve ser na primeira torre para uma torre vazia
                if (Attemps == 3)
                    AlterPopAndPushIfFourth(ref randomTowerPop, ref randomTowerPush);

                MakeMovement(randomTowerPop, randomTowerPush);
                Attemps = Attemps + 1;

                continue;
            }

            AlterIfPopAndPushIsEquals(ref randomTowerPop, randomTowerPush);
            AlterPopIfBothIsEmpty(ref randomTowerPop, randomTowerPush);
            AlterPopIfEmpty(ref randomTowerPop, randomTowerPush);
            AlterPushIfFulled(ref randomTowerPush, randomTowerPop);
            AlterPopIfIsBigger(ref randomTowerPush, ref randomTowerPop);

            AlterPopAndPushIfOneDiskIsMissingForLastTower(ref randomTowerPop, ref randomTowerPush);

            //o menor disco nunca pode fica como primeiro disco de uma das torres, exeto se um das torrers conter todos os discos
            AlterPopAndPushIfFistDiskFromPushIsSmaller(ref randomTowerPop, ref randomTowerPush);

            //verificar se a torre 3 esta vazia e se há uma torre com um unico disco que seja do maior tamanho. Colocar esse diso maior na torre 3
            MoveBiggerDiskToTower3IfIsEmpty(ref randomTowerPop, ref randomTowerPush);

            //se o menor disco esta no topo de alguma torre, esse disco é o que precisa ser movido (pop)
            //AlterPopAndPushIfSmallerDiskOnTheTop(ref randomTowerPop, ref randomTowerPush);

            //verifica se o disco que esta sendo retirado é maior que o disco que esta na torre de destino
            if(AlterIfDiskIntoTowerIsSmallerThanPop(randomTowerPop, randomTowerPush))
                continue;
            
            //fazer essa tarefa dentro do objeto
            MakeMovement(randomTowerPop, randomTowerPush);

            Attemps++;
        }
    }

    private void MakeMovement(int randomTowerPop, int randomTowerPush)
    {
        var movement = _towers[randomTowerPop].Remove();
        if (movement.registerMovement != ERegisterMovement.Attached)
            return;

        Console.WriteLine(
            $"Removendo o disco {movement.towePop} da torre {_towers[randomTowerPop].TowerName} e Inserindo na torre {_towers[randomTowerPush].TowerName}");
        _towers[randomTowerPush].Add(movement.towePop);
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

        var tower = _towers.First(x => x.Disks.Count > 0 && x.Peek() == (_disksOrder.First() * 10));
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

        if (_towers[randomTowerPop].Peek() != (_disksOrder.Last() * 10))
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

        if (_towers[randomTowerPop].HasDisks() && _towers[randomTowerPop].Peek() == _disksOrder.First())
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
        if (!_towers[2].IsEmpty())
            return;

        //Busca a possivel torre cujo o disco do topo seja o maior disco
        var tower = _towers.FirstOrDefault(x => x.Disks.Count > 0 && x.Peek() == (_disksOrder.Last() * 10));

        if (tower is null)
            return;

        randomTowerPush = 2;
        randomTowerPop = _towers.IndexOf(tower);
    }

    private void AlterPopAndPushIfOneDiskIsMissingForLastTower(ref int randomTowerPop, ref int randomTowerPush)
    {
        if (_towers[2].Disks.Count + 1 < _disksOrder.Count)
            return;

        //se for tirar do 2 ou o disco de onde for tirar for 2 não é o menor disco, entao sai
        if ((randomTowerPop == 2) || (_towers[randomTowerPop].Peek() != (_disksOrder.First() * 10)))
            return;

        var options = new List<int> { 0, 1, 2 };

        randomTowerPush = 2;
        options.Remove(2);

        if (_towers[options.First()].Disks.Count > 0)
            randomTowerPop = options.First();
        else
            randomTowerPop = options.Last();
    }

    private bool AlterIfDiskIntoTowerIsSmallerThanPop(int randomTowerPop, int randomTowerPush)
    {
        return _towers[randomTowerPush].IfLastDiskSmaller(_towers[randomTowerPop].Peek());


        // var optionsPush = new List<int> { 0, 1, 2 };
        // var optionsPop = new List<int> { 0, 1, 2 };
        // var options = new List<int> { 0, 1, 2 };
        //
        // if (_towers[randomTowerPop].Disks.Count == 0)
        //     return;
        //
        // if (_towers[randomTowerPush].IfLastDiskSmaller(_towers[randomTowerPop].Peek()) == false)
        //     return;
        //
        // //veja se o disco da torre que ira receber tem o menor disco que o os outros discos das outras torres
        // //sendo que não pode haver torres vazias
        // //e que a mesma torre nao tenho o menor e o maior disco
        // var towerDiskPush = _towers[randomTowerPush].Peek();
        // options.Remove(randomTowerPush);
        //
        // var itsSmallerDisk = towerDiskPush < _towers[options.First()].Peek() && towerDiskPush < _towers[options.Last()].Peek();
        //
        // if ( (_towers.Any(x => x.IsEmpty()) == false) && itsSmallerDisk &&
        //      (_towers[randomTowerPush].Disks.Contains(_disksOrder.First() * 10) &&  _towers[randomTowerPush].Disks.Contains(_disksOrder.Last() * 10) == false)
        //      )
        // {
        //     //se for,
        //     //essa torre nao podera ser o push
        //     optionsPush.Remove(randomTowerPush);
        //
        //     //encontre a torre com o maior disco (nao pode ser a torre que tem o menor disco), essa torre não poderá ser o pop
        //     var biggerDiskTower =
        //         _towers.FirstOrDefault(x => x.Disks.Count > 0 && x.Peek() == (_disksOrder.Last() * 10));
        //     if (biggerDiskTower is not null)
        //     {
        //         optionsPop.Remove(_towers.IndexOf(biggerDiskTower));
        //     }
        //
        //     //ira sobrar duas alternativas. 
        //     //a torre com o maior disco não receber o menor disco
        //     //colocar o menor disco na torre maior (o que acontece na maioria das vezes)
        //
        //     if (new Random().Next(0, 11) < 7)
        //     {
        //         randomTowerPop = randomTowerPush;
        //
        //         optionsPop.Remove(_towers.IndexOf(biggerDiskTower!));
        //         randomTowerPush = _towers.IndexOf(biggerDiskTower!);
        //     }
        //     else
        //     {
        //         randomTowerPop = randomTowerPush;
        //         randomTowerPush = _towers.IndexOf(biggerDiskTower!);
        //     }
        // }
        // else if (itsSmallerDisk)
        // {
        //     optionsPush.Remove(randomTowerPush);
        // }
        // else
        // {
        //     //Senao
        //     //Se o primeiro disco da pilha do push for menor que o disco que está sendo inserido, não pode colocar la
        //     //Nem colocar de onde estiver tirando o disco
        //     options.Remove(randomTowerPush);
        //     options.Remove(randomTowerPop);
        //     
        //     randomTowerPop = options.First();
        // }
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