namespace ToweOfHanoi;

public class Tower
{
    public Stack<int> Disks { get; private set; } = new();
    public int NumberOfDisksFromGame { get; private set; }
    public string TowerName { get; private set; }

    public Tower(int numberOfDisks, string towerName) => 
        (NumberOfDisksFromGame, TowerName) = (numberOfDisks, towerName);

    public ERegisterMovement Add(int disk)
    {
        if (Disks.Count == NumberOfDisksFromGame)
            return ERegisterMovement.Fulled;

        if (Disks.Count < NumberOfDisksFromGame)
        {
            Disks.Push(disk);
            return ERegisterMovement.Attached;
        }

        if (Disks.Peek() < disk)
            return ERegisterMovement.LastDiskSmaller;
        
        throw new InvalidOperationException("Nenhuma validação foi feita.");
    }

    public (ERegisterMovement registerMovement, int towePop) Remove() =>
        Disks.Count == 0 ? (ERegisterMovement.Empty, 0) : (ERegisterMovement.Attached, Disks.Pop());

    public bool Verify()
    {
        if((Disks.Count == 0) || (Disks.Count < NumberOfDisksFromGame))
            return false;
        
        int lastDisk = 0;
        int cont = 0;

        foreach (var disk in Disks)
        {
            if (cont > 0)
            {
                if (lastDisk > disk)
                {
                    throw new InvalidDataException("A torre nao esta ordenada.");
                }
            }
         
            lastDisk = disk;
            cont++;
        }
        
        for (int i = 0; i < Disks.Count; i++)
        {
            lastDisk = Disks.Peek();
            if (i == 0)
                continue;
            
            if(lastDisk < Disks.Peek())
                throw new InvalidDataException("A torre não está ordenada.");
        }
        
        return true;
    }
    
    public bool IsEmpty() =>
        Disks.Count == 0;
    
    public bool IsFull() =>
        Disks.Count == NumberOfDisksFromGame;
    
    public bool HasDisks() =>
        Disks.Count > 0;

    public bool IfLastDiskSmaller(int disk)
    {
        if(Disks.Count == 0)
            return false;

        if (Disks.Peek() < disk)
            return true;
        
        return false;
    }

    public bool IfMissingJustOneDisk()
    {
        if(Disks.Count + 1 == NumberOfDisksFromGame)
            return true;
        
        return false;
    }

    public int Peek()
    {
        if(Disks.Count == 0)
            throw new InvalidOperationException("Tower is empty");
        
        return Disks.Peek();
    }
}