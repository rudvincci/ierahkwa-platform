using System.Numerics;

namespace Mamey.Web3.Messages;

public class NewBlock
{
    public NewBlock(BigInteger blockNumber)
    {
        BlockNumber = blockNumber;
    }

    public BigInteger BlockNumber { get; }
}