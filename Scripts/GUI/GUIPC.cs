using Godot;
using GDC = Godot.Collections;
using System;

public class GUIPC : Control {

	[Export]
	public Resource inboxResource;
	[Export]
	public bool updateInbox = false;

	private Inbox inbox;
	private int selectedMail = 0;

	public struct Inbox {
		public Mail[] mails;
		public Inbox(Mail[] mails) {
			this.mails = mails;
		}
	}

	public struct Mail {
		public String from;
		public String to;
		public String text;
		public bool isRead;
		public Mail(String from, String to, String text, bool isRead) {
			this.from = from;
			this.to = to;
			this.text = text;
			this.isRead = isRead;
		}
	}

	private Theme pcTheme = ResourceLoader.Load<Theme>("res://Assets/Themes/ThemePC.tres");
	private StreamTexture texUnread = ResourceLoader.Load<StreamTexture>("res://Assets/Sprites/GUI/PC/spr_pc_newmessage.png");
	private DynamicFont font = ResourceLoader.Load<DynamicFont>("res://Assets/Fonts/6pxFont.tres");

	private Label labelTo;
	private Label labelFrom;
	private RichTextLabel labelText;

	private VBoxContainer mailsHolder;

	private GDC.Array<Button> mailArr = new GDC.Array<Button>();

	public override void _Ready() {
		labelTo = GetNode<Label>("CenterContainer/Control/Panel/Main/FieldTo/LabelTo");
		labelFrom = GetNode<Label>("CenterContainer/Control/Panel/Main/FieldFrom/LabelFrom");
		labelText = GetNode<RichTextLabel>("CenterContainer/Control/Panel/Main/MailText/LabelText");
		mailsHolder = GetNode<VBoxContainer>("CenterContainer/Control/Panel/Main/MailBox/MailsHolder");

		UpdateMail();
	}

	public override void _Process(float delta) {
		if (updateInbox) {
			updateInbox = false;
			try {
				UpdateMail();
			} catch (Exception e) {
				GD.PrintErr(e.ToString());
			}
		}
	}

	public void UpdateMail() {
		if (inboxResource is null) return;
		GDC.Array arr = (GDC.Array) inboxResource.Get("mail");
		Mail[] mails = new Mail[arr.Count];

		foreach (Godot.Node obj in mailsHolder.GetChildren()) {
			mailsHolder.RemoveChild(obj);
		}
		

		for (int i = 0; i < arr.Count; i ++) {
			Resource res = (Resource) arr[i];
			String from = (String) res.Get("from");
			String to = (String) res.Get("to");
			String text = (String) res.Get("text");
			bool isRead = (bool) res.Get("isRead");
			Mail mail = new Mail(from, to, text, isRead);
			mails.SetValue(mail, i);
			//GD.Print(from, " \n", to, " \n", text, " \n", isRead, "\n\n");

			Button but = new Button();
			mailsHolder.AddChild(but);
			mailArr.Add(but);
			but.Text = from;
			but.Icon = isRead ? null : texUnread;
			but.Theme = pcTheme;
			but.Set("custom_fonts/font", font);
			but.Connect("pressed", this, nameof(ShowMail), new GDC.Array(){i});
		}

		inbox = new Inbox(mails);

		if (arr.Count > 0) {
			ShowMail(0);
		}
	}  

	public void ShowMail(int ind) {
		ind = Mathf.Clamp(ind, 0, inbox.mails.Length - 1);
		Mail mail = inbox.mails[ind];
		ReadMail(mail, ind);
		labelFrom.Text = mail.from;
		labelTo.Text = mail.to;
		labelText.BbcodeText = mail.text;
	}

	public void ReadMail(Mail mail, int ind) {
		mail.isRead = true;
		mailArr[ind].Icon = null;
		GDC.Array arr = (GDC.Array) inboxResource.Get("mail");
		Resource res = (Resource) arr[ind];
		res.Set("isRead", true);
	}
}
