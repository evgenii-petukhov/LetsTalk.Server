using LetsTalk.Server.SignPackage.Abstractions;
using LetsTalk.Server.UnitTests.Models;
using LetsTalk.Server.UnitTests.Models.Signable;

namespace LetsTalk.Server.UnitTests.TestCases;

public static class SignPackageServiceTestCases
{
    private const string SampleSignature = "58319BC4AEF8F536689876EF9D90FC0A";

    public static readonly object[] ObjectsToSign =
    [
        new TestData<ISignable, string>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B"
            },
            Result = SampleSignature
        },
        new TestData<ISignable, string>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true
            },
            Result = SampleSignature
        },
        new TestData<ISignable, string>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true
            },
            Result = SampleSignature
        },
        new TestData<ISignable, string>
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                E = []
            },
            Result = SampleSignature
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
                Signature = SampleSignature
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
                Signature = SampleSignature
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
                Signature = SampleSignature
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
                Signature = SampleSignature
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
