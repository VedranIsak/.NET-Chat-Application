using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

/*
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    Communicator communicator;
    Boolean isChatting = false;
    public User Person { get; set; }
    Thread recieveMessageThread = null;

    private ObservableCollection<MessageObj> messageObjs = new ObservableCollection<MessageObj>();

    public MainWindow()
    {
        //InitializeComponent();
        this.Person = new User();
        this.communicator = new Communicator();
        //DataContext = this.jag;
        DataContext = this;
    }
        

    //  https://www.youtube.com/watch?v=XXg9g56FS0k

    public void inviteToChat(object sender, EventArgs e)
    {
            
        try
        {
            if (this.recieveMessageThread != null)
            {
                try
                {
                    this.stopChat(new object(), new EventArgs());
                }
                catch(IOException err)
                {
                    Console.WriteLine(err);
                }
            }
            this.communicator.ConnectToOtherPerson(port: Person.Port, server: Person.Ip, name: this.Person.Name);
            this.isChatting = true;
        }
        catch(SocketException exception)
        {
            this.isChatting = false;
            Console.WriteLine(exception);
            MessageBox.Show("Nobody there to connect to");
        }
        if (this.isChatting)
        {
            this.recieveMessageThread = new Thread(() =>
            {
                MessageObj message = null;
                while (true)
                {
                    try
                    {
                        message = this.communicator.recieveMessage();
                    }
                    catch (Newtonsoft.Json.JsonReaderException err)
                    {
                        Console.WriteLine(err);
                    }
                    if (message != null && message.MessageType == "disconnect")
                    {
                        break;
                    }
                    this.mes.Dispatcher.Invoke(() =>
                    {
                        this.messageObjs.Add(message);
                        this.mes.Text = message.Sender + " : " + message.Message;
                        for (int i = 0; i < this.messageObjs.Count; i++)
                        {
                            Console.WriteLine(this.messageObjs[i].Message);
                        }
                    });
                }
            });
            this.recieveMessageThread.IsBackground = true;
            this.recieveMessageThread.Start(); 
        }
    }

    public void listenForChat(object sender, EventArgs e)
    {
        // Need to use threads or the program stops working here
        Console.WriteLine("Listen for chat");
        if (this.recieveMessageThread != null)
        {
            try
            {
                this.stopChat(new object(), new EventArgs());
            }
            catch (IOException err)
            {
                Console.WriteLine(err);
            }
        }
        this.communicator.ListenToPort(port: Person.Port, name: this.Person.Name);
        this.recieveMessageThread = new Thread(() =>
        {
            MessageObj message = null;
            while(true)
            {
                try
                {
                    message = this.communicator.recieveMessage();
                }
                catch(Newtonsoft.Json.JsonReaderException err)
                {
                    Console.WriteLine(err);
                }
                if (message != null && message.MessageType == "disconnect")
                {
                    break;
                }
                this.mes.Dispatcher.Invoke(() =>
                {
                    this.messageObjs.Add(message);
                    this.mes.Text = message.Sender + " : " + message.Message;

                    for (int i = 0; i < this.messageObjs.Count; i++)
                    {
                        Console.WriteLine(this.messageObjs[i].Message);
                    }
                });
            }
        });
        this.recieveMessageThread.IsBackground = true;
        this.recieveMessageThread.Start();
            
    }

    public void sendMessage(object sender, EventArgs e)
    {
        this.communicator.sendMessage(this.Person.Name, "message", this.Meddelande.Text);
        Console.WriteLine("Send Message");
    }

    public void disconnect ()
    {
        this.recieveMessageThread.Abort();
        this.recieveMessageThread = null;
        this.communicator.disconnectStream();
    }
    public void stopChat(object sender, EventArgs e)
    {
        Console.WriteLine("Stop chat");
        if(recieveMessageThread != null)
        {
            this.recieveMessageThread.Abort();

        }
        this.recieveMessageThread = null;
        this.communicator.sendMessage(this.Person.Name, "disconnect");
        this.communicator.stopChatting(this.Person.Name);
    }
        
}
    */
