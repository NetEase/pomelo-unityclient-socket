using System;


public class ChatRecord {
    public string name { get; set; }
    public string dialog { get; set; }

    public ChatRecord() {

    }
    //Chat record
    public ChatRecord(string userName, string userDialog) {
        this.name = userName;
        this.dialog = userDialog;
    }
}