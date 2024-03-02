namespace LetsTalk.Server.UnitTests.Services.TestCases;

public static class SignPackageServiceTestCases
{
    public static readonly object[] ObjectsToSign = new object[]
    {
        new SignTestData
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B"
            },
            Result = "BBCD0166BC1AA9C0D18D875C740ECD35"
        },
        new SignTestData
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true
            },
            Result = "BBCD0166BC1AA9C0D18D875C740ECD35"
        },
        new SignTestData
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                D = true
            },
            Result = "BBCD0166BC1AA9C0D18D875C740ECD35"
        },
        new SignTestData
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                E = new List<string>()
            },
            Result = "BBCD0166BC1AA9C0D18D875C740ECD35"
        }
    };

    public static readonly object[] ObjectsToValidate = new object[]
    {
        new ValidateTestData
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                Signature = "BBCD0166BC1AA9C0D18D875C740ECD35"
            },
            Result = true
        },
        new ValidateTestData
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
        new ValidateTestData
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
        new ValidateTestData
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                E = new List<string>(),
                Signature = "BBCD0166BC1AA9C0D18D875C740ECD35"
            },
            Result = true
        },
        new ValidateTestData
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B"
            },
            Result = false
        },
        new ValidateTestData
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
        new ValidateTestData
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
        new ValidateTestData
        {
            Value = new SimpleSignable
            {
                A = 1,
                B = "B",
                E = new List<string>(),
                Signature = "b"
            },
            Result = false
        }
    };
}
