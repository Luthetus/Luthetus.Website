namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts;

public partial class ReplStateFacts
{
    public static readonly string APP_TS_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/wwwroot/ts/app.ts";
    
    public static readonly string APP_TS_CONTENTS = @"interface User {
  name: string;
  id: number;
}
 
class UserAccount {
  name: string;
  id: number;
 
  constructor(name: string, id: number) {
    this.name = name;
    this.id = id;
  }
}
 
const user: User = new UserAccount(""Murphy"", 1);";
}
