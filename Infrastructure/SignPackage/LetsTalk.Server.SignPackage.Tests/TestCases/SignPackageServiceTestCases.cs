using LetsTalk.Server.SignPackage.Tests.Models.Signable;

namespace LetsTalk.Server.SignPackage.Tests.TestCases;

public static class SignPackageServiceTestCases
{
    public static readonly object[] ObjectsToSign =
    [
        new object[] {
            new SimpleSignable
            {
                A = 1,
                B = "B"
            }
        },
        new object[] {
            new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true
            }
        },
        new object[] {
            new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true
            }
        },
        new object[] {
            new SimpleSignable
            {
                A = 1,
                B = "B",
                E = []
            }
        }
    ];
}
