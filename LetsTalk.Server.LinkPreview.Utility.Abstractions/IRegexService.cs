﻿using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.Utility.Abstractions;

public interface IRegexService
{
    OpenGraphModel GetOpenGraphModel(string input);
}
