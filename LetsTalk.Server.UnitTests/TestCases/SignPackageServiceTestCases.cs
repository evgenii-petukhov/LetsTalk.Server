using LetsTalk.Server.SignPackage.Abstractions;
using LetsTalk.Server.UnitTests.Models;
using LetsTalk.Server.UnitTests.Models.Signable;

namespace LetsTalk.Server.UnitTests.TestCases;

public static class SignPackageServiceTestCases
{
    public static readonly object[] ObjectsToSign =
    [
        new TestData<ISignable, string>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B"
            },
            Result = "BBCD0166BC1AA9C0D18D875C740ECD35"
        },
        new TestData<ISignable, string>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true
            },
            Result = "BBCD0166BC1AA9C0D18D875C740ECD35"
        },
        new TestData<ISignable, string>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true
            },
            Result = "BBCD0166BC1AA9C0D18D875C740ECD35"
        },
        new TestData<ISignable, string>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                E = []
            },
            Result = "BBCD0166BC1AA9C0D18D875C740ECD35"
        }
    ];

    public static readonly object[] ObjectsToValidate =
    [
        new TestData<SimpleSignable, bool>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                Signature = "BBCD0166BC1AA9C0D18D875C740ECD35"
            },
            Result = true
        },
        new TestData<SimpleSignable, bool>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true,
                Signature = "BBCD0166BC1AA9C0D18D875C740ECD35"
            },
            Result = true
        },
        new TestData<SimpleSignable, bool>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true,
                Signature = "BBCD0166BC1AA9C0D18D875C740ECD35"
            },
            Result = true
        },
        new TestData<SimpleSignable, bool>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                E = [],
                Signature = "BBCD0166BC1AA9C0D18D875C740ECD35"
            },
            Result = true
        },
        new TestData<SimpleSignable, bool>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B"
            },
            Result = false
        },
        new TestData<SimpleSignable, bool>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true,
                Signature = string.Empty
            },
            Result = false
        },
        new TestData<SimpleSignable, bool>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true,
                Signature = "a"
            },
            Result = false
        },
        new TestData<SimpleSignable, bool>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                E = [],
                Signature = "b"
            },
            Result = false
        }
    ];
}
