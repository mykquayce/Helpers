﻿using Microsoft.Extensions.Options;

namespace Helpers.GlobalCache.Models;

public class MessagesDictionary : Dictionary<string, string>, IOptions<MessagesDictionary>
{
	public MessagesDictionary Value => this;
}
