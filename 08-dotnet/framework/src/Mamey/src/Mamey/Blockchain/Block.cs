using System.Security.Cryptography;
using System.Text;

namespace Mamey.Blockchain
{
    public interface IBlock
    {
        int Index { get; }
        DateTime TimeStamp { get; }
        string PreviousHash { get; }
        string Hash { get; }
        string Data { get; }

        bool IsValid();
        IBlock PreviousBlock { get; }
    }

    public interface IBlockChain
    {
    }
    internal class Block : IBlock
    {
        public Block(DateTime timeStamp, string previousHash, string data)
        {
            Index = 0;
            TimeStamp = timeStamp;
            PreviousHash = previousHash;
            Data = data;
            Hash = CalculateHash();
        }

        public int Index { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public string Data { get; set; }

        public IBlock PreviousBlock => throw new NotImplementedException();

        public string CalculateHash()
        {
            var sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.ASCII
                .GetBytes($"{TimeStamp}-{PreviousHash ?? ""}{Data}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);
            return Convert.ToBase64String(outputBytes);
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }

    internal class Blockchain
    {
        public Blockchain()
        {
            InitializeBlockchain();
            AddGenesisBlock();
        }

        public IList<Block> Chain { get; set; }

        private void InitializeBlockchain()
        {
            Chain = new List<Block>();
        }

        private Block CreateGenesisBlock()
        {
            return new Block(DateTime.Now, null, "{}");
        }

        private void AddGenesisBlock()
        {
            Chain.Add(CreateGenesisBlock());
        }

        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = GetLatestBlock();
            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;
            block.Hash = block.CalculateHash();
            Chain.Add(block);
        }

        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                var currentBlock = Chain[i];
                var previousBlock = Chain[i - 1];
                if(currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }
        

    }
}

