using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Impact",
                 "Impact",
                 "Assets/10.jpg",
                 "A tattoo is a form of body modification, made by inserting indelible ink into the dermis layer of the skin to change the pigment. The first written reference to the word, tattoo or Samoan Tatau appears in the journal of Joseph Banks, the naturalist aboard Captain Cook's ship the HMS Endeavour: I shall now mention the way they mark themselves indelibly, each of them is so marked by their humor or disposition.Golf is a precision club and ball sport in which competing players (or golfers) use many types of clubs to hit balls into a series of holes on a course using the fewest number of strokes. Golf is defined, in the rules of golf, as playing a ball with a club from the teeing ground into the hole by a stroke or successive strokes in accordance with the Rules.");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "What is Tattoo?",
                 "What is Tattoo?",
                 "Assets/11.jpg",
                 "A tattoo is a form of body modification, made by inserting indelible ink into the dermis layer of the skin to change the pigment. The first written reference to the word, tattoo (or Samoan Tatau) appears in the journal of Joseph Banks, the naturalist aboard Captain Cook's ship the HMS Endeavour.",
                 "A tattoo is a form of body modification, made by inserting indelible ink into the dermis layer of the skin to change the pigment. The first written reference to the word, tattoo or Samoan Tatau appears in the journal of Joseph Banks, the naturalist aboard Captain Cook's ship the HMS Endeavour: I shall now mention the way they mark themselves indelibly, each of them is so marked by their humor or disposition.The word tattoo was brought to Europe by the explorer James Cook, when he returned in 1771 from his first voyage to Tahiti and New Zealand. In his narrative of the voyage, he refers to an operation called tattaw. Before this it had been described as scarring, painting, or staining. Whole back tattoo, USA 2007 \n\nA trompe-l'œil spider tattoo \nTattooing has been practiced for centuries in many cultures, particularly in Asia, and spread throughout the world.[citation needed] The Ainu, an indigenous people of Japan, traditionally had facial tattoos. Today, one can find Atayal, Seediq, Truku, and Saisiyat of Taiwan, Berbers of Tamazgha (North Africa), Yoruba, Fulani and Hausa people of Nigeria, and Māori of New Zealand with facial tattoos.[citation needed Tattooing was widespread among Polynesians and among certain tribal groups in Africa, Borneo, Cambodia, Europe, Japan, the Mentawai Islands, MesoAmerica, New Zealand, North America and South America, the Philippines, and Taiwan.[2] Indeed, the island of Great Britain takes its name from tattooing; Britons translates as people of the designs, and Picts, the peoples who originally inhabited the northern part of Britain, literally means the painted people. Despite some taboos surrounding tattooing, the practice continues to be popular in many parts of the world.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Types",
                 "Types",
                 "Assets/12.jpg",
                 "According to George Orwell, coal miners could develop characteristic tattoos owing to coal dust getting into wounds.[13] This can also occur with substances like gunpowder. Similarly, a traumatic tattoo occurs when a substance such as asphalt is rubbed into a wound as the result of some kind of accident or trauma.",
                 "Amateur and professional tattoos\n\nTattooing among females of the Koita people of Papua New Guinea traditionally began at age five and was added to each year, with the V-shaped tattoo on the chest indicating that she had reached marriageable age, 1912. Many tattoos serve as rites of passage, marks of status and rank, symbols of religious and spiritual devotion, decorations for bravery, sexual lures and marks of fertility, pledges of love, punishment, amulets and talismans, protection, and as the marks of outcasts, slaves and convicts. The symbolism and impact of tattoos varies in different places and cultures. Tattoos may show how a person feels about a relative (commonly mother/father or daughter/son) or about an unrelated person. Today, people choose to be tattooed for artistic, cosmetic, sentimental/memorial, religious, and magical reasons, and to symbolize their belonging to or identification with particular groups, including criminal gangs (see criminal tattoos) or a particular ethnic group or law-abiding subculture. Some Māori still choose to wear intricate moko on their faces. In Cambodia, Laos, and Thailand, the yantra tattoo is used for protection against evil and to increase luck.citation neededIn the Philippines certain tribal groups believe tattoos have magical qualities, and help to protect their bearers. Most traditional tattooing in the Philippines is related to the bearer's accomplishments in life or rank in the tribe.[citation needed Extensive decorative tattooing is common among members of traditional freak shows and by performance artists who follow in their tradition.\n\nIdentification\n\n Tattoo marking a deserter from the British Army. Skin removed post-mortem. People have also been forcibly tattooed. A well-known example is the identification system for inmates in Nazi concentration camps during the Holocaust. Tattoos have also been used for identification in other ways.[citation needed] For example, during the Roman Empire, Roman soldiers were required by law to have identifying tattoos on their hands in order to make it difficult to hide if they deserted. Gladiators and slaves were likewise tattooed, exported slaves were tattooed with the words tax paid, and it was a common practice to tattoo Stop me, I'm a runaway on their foreheads. Emperor Constantine I banned tattooing the face around AD 330 and the Second Council of Nicaea banned all body markings as a pagan practice in AD 787\n\nIn the period of early contact between the Māori and Europeans, the Maori people were hunted for their moko tattoos and decapitated to provide souvenirs of the New World.[citation needed] Moko tattoos were facial designs worn by women and men to indicate their lineage, social position, and status within the tribe. The tattoo art was a sacred marker of identity among the Maori and also referred to as a vehicle for storing one's tapu, or spiritual being, in the afterlife. \n\nTattoos are sometimes used by forensic pathologists to help them identify burned, putrified, or mutilated bodies. As tattoo pigment lies encapsulated deep in the skin, tattoos are not easily destroyed even when the skin is burned.\n\nAn identification tattoo on a survivor of the Auschwitz concentration camp.\n\nTattoos are also placed on animals, though rarely for decorative reasons. Pets, show animals, Thoroughbred horses, and livestock are sometimes tattooed with identification and other marks. Pet dogs and cats are often tattooed with a serial number (usually in the ear, or on the inner thigh) via which their owners can be identified. \n\nAlso, animals are occasionally tattooed to prevent sunburn (on the nose, for example). Such tattoos are often performed by a veterinarian, and in most cases the animals are anesthetized during the process. Branding is used for similar reasons and is often performed without anesthesia, but is different from tattooing as no ink or dye is inserted during the process\n\nCosmetic\n\nThe cosmetic surgery industry continues to see a trend of increased popularity for both surgical and noninvasive procedures (Gimlin 2002; Sullivan 2001).\n\nTattooed lip makeup Main article: Permanent makeup\nWhen used as a form of cosmetics, tattooing includes permanent makeup and hiding or neutralizing skin discolorations. Permanent makeup is the use of tattoos to enhance eyebrows, lips (liner and/or lipstick), eyes (liner), and even moles, usually with natural colors, as the designs are intended to resemble makeup.[citation needed]\n\nMedical\n\nMain article: Medical tattoo Medical tattoos are used to ensure instruments are properly located for repeated application of radiotherapy and for the areola in some forms of breast reconstruction. Tattooing has also been used to convey medical information about the wearer (e.g., blood group, medical condition, etc.). Additionally, tattoos are used in skin tones to cover vitiligo, a skin pigmentation disorder.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "Best Way to Design",
                 "Best Way to Design",
                 "Assets/13.jpg",
                 "Don't Settle for Less Than the Best Finding the best tattoo designs is extremely important. There are many designs out there that are just poor quality, and will not be right for you.",
                 "Don't Settle for Less Than the Best Finding the best tattoo designs is extremely important. There are many designs out there that are just poor quality, and will not be right for you. Nothing is worse than choosing a bad design, or working with an artist that produces a bad design. This is a decision you want to take lightly, so finding the best designs is key. What you need to find are top quality designs that are submitted by real artists. Make sure you like the look of what the artist has produced on paper, because that is what they will look like on your skin. Also, be sure to check the references of the artist that is working on you. Take a look at past work they have done to ensure that they'll be able to match what's on paper, or produce the right, unique design for you.\n\nA good way to avoid problems is to find the right tattoo design gallery on the web. There are free ones out there, but you have to be careful in terms of quality. The paid ones often have the best designs because the artists are truly dedicated to their work, and the selection is vast. \n\nOnce you join one of these galleries, you'll simply save your favorites, and print them off to make a final decision. Let these thoughts marinate for a few days so you can be sure you're making the right choice for you. It can be tough if there are many different ones you're considering, so ask friends and family members what they think. More often than not, inspiration will hit and you'll know what the best tattoo design is for you. When you make your final decision, consider whether there is any part of the tattoo you don't like. Some people will want to have a name, or wording, or even merge two different pictures together. Contract your tattoo artist to be sure this can be done. Actually, some of the premium Galleries make this easier than ever since you can merge two pictures right on the screen! This is a great way to personalize things yourself and come out with a unique tattoo that no one else will have.\n\nMake sure you're comfortable with your final decision. If you're having any reservations, you might want to hold off on getting a tattoo. Once you feel right with the tattoo and are sure it is what you want, it will be something you treasure for the rest of your life. The quality will come through with the right design, and the right artist. And it all starts with finding the best tattoo designs!",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "First Step",
                 "First Step",
                 "Assets/14.jpg",
                 "First Steps to Getting a Tattoo Getting a tattoo is a decision that should never be taken lightly. This is something that will stay with you for the rest of your life! With that being said, it is also one of the best ways to express yourself",
                 "First Steps to Getting a Tattoo Getting a tattoo is a decision that should never be taken lightly. This is something that will stay with you for the rest of your life! With that being said, it is also one of the best ways to express yourself, and show off who you truly are. There are an incredible number of great reasons to get a tattoo, but the fact that you're reading this shows that you already know that! It's time to take your first steps to getting a tattoo! The first thing you need to do is examine your reason for getting the tattoo. Perhaps you want to get one in honor of someone else, because you like a certain design, or because you just like the looks of them. No matter what your reason, you need to be sure you realize what it is so you end up with the perfect design. \n\nAfter you've thought about your reasons, it's time to choose the actual design. There are many different ways to find the design that is right. You can browse your local tattoo artist's shop, or, better yet, search through one of the online galleries. These are neat because they feature the work of great artists from all around the world. \n\nOnce you've chosen the design, or at least gotten a rough idea, it's time to take it to your local artist. Be careful! Don't just go to any artist you find. You need to find someone with a good reputation. Ask to see examples of past work. Better yet, ask friends and family members for referrals to artists they've been happy with. You'll need a great artist, who works in a clean and sterile environment. After you've chosen the perfect artist you should work with him or her on a consultation. Bring in any designs you are considering, because they may be able to help you choose the one that is right for you. Be sure to discuss if you want anything added or taken away from the design. Always get these changes down on paper, because what you're envisioning, and what the artist is envisioning might be two different things!\n\nFinally! After you have taken these first steps, you'll be ready to get your tattoo. It will be a great feeling to finally have the right design. It will be something you can cherish forever, and that other people can admire. It all starts with these simple first steps.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Printable Tattoo",
                 "Printable Tattoo",
                 "Assets/15.jpg",
                 "Printable Tattoo Designs are Easy to Get Printable tattoo designs are a great thing for those getting tattoos. It used to be that you would go to your tattoo artist and browse through their various design books.",
                 "Printable Tattoo Designs are Easy to Get Printable tattoo designs are a great thing for those getting tattoos. It used to be that you would go to your tattoo artist and browse through their various design books. These days, you can find the right design from the comforts of your home, and not have to make a snap decision. You also don't have to make multiple trips to the tattoo artists just to browse through their books if you don't want to make a quick decision! The first thing you'll need to do is find a gallery. There are many online -- some are free and some are paid. While it's possible that you can find the right design amongst the free selection, you'll probably find a lot more you like by paying a small fee to gain access to a premium selection. After all, the tattoo is something that will be with you for the rest of your life. Finding the right design is essential, and you can't do that unless you have the best selection from the best artists!\n\nThe galleries you gain access to will likely be set up into different categories. Choose the category that you are looking for, and browse through. Pay special attention to the quality, and the way the artist portrays the design. Many galleries are collected from various artists, so you'll likely find one that appeals to you.\n\nThen, you want to enlarge the design so it is the right quality to print out. The premium galleries offer printable tattoo designs that will be excellent quality. Make sure you have high-quality paper and ink as well, and then print on the highest settings. That way, none of the detail will be left out. Take it to your tattoo artist so he can easily give you exactly what you're looking for.\n\nSometimes, you may find a couple of tattoo designs that will work well together. A good tattoo artist can incorporate two different designs! You'll want to print these out (some galleries allow you to incorporate them right on-screen!) and take them to your artist. A good artist will also be able to tweak the design if you have something specific you want done to it.\n\nThe result of all this is that you'll have the perfect tattoo for you. It's only possible using printable tattoo designs, which are quite possibly the best thing to happen to those who get tattoos in quite some time.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Unique Tattoo",
                 "Unique Tattoo",
                 "Assets/16.jpg",
                 "Stand Out From the Crowd! Nothing is worse than having the same tattoo as everyone else! That's why finding unique tattoo designs is more important than ever these days.",
                 "Stand Out From the Crowd! Nothing is worse than having the same tattoo as everyone else! That's why finding unique tattoo designs is more important than ever these days. Thankfully, there are galleries full of thousands of tattoos that you can use for inspiration to find your own unique tattoo design. You can even incorporate different designs together to make your own even more unique. If you're lucky enough to know an artist, you can work with them so they'll design the perfect tattoo for you. Since this is something that will come from your own mind, it will be absolutely unique to you. It's great if you're friends with an artist who'll do it for free, but more often than not you'll be charged a hefty fee for this on top of getting the actual tattoo.\n\nMany people will have more luck finding the right galleries. There are free ones out there, but you can be sure that other people using the free ones will get the same tattoos -- they certainly aren't unique! It is far better to go with one of the premium Galleries that offers thousands upon thousands of different tattoos, and allows them to be customized. Sure, you'll pay a small fee, but it is well worth it so you'll have a unique tattoo design.\n\nInstead of hiring an artist to come up with a completely new design for you, you can take one of the designs you find in these galleries and have your artist tweak it. He can add lettering and certain details, or eliminate details that you don't want. A good artist will work with you on creating the perfect tattoo that fits your personal style. As was briefly mentioned, one of the best ways to do this is to add names or wording. This can be your own name, that of a friend or family member, the name of someone who has passed away, scripture, words, and many other options. This will add a level of personalization that will certainly make your tattoo unique.\n\nThere is no reason to follow the crowd when it comes to getting a tattoo. Everyone gets tired of seeing the same little butterfly put on the small of so many women's backs, and the same topless woman on a man's biceps! It's time to add your own style and flair into the mix and get something that will be truly unique. Not only will people admire you, but you'll feel more attached to the tattoo as something that is uniquely yours. Don't go with the flow; get something better!",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Gallery",
                 "Gallery",
                 "Assets/20.jpg",
                 "Finding a great tattoos gallery is essential if you want to find the right design for you. This is not a decision that you should take lightly, and it should certainly not be made on the spot when you're about to enter the artist's chair! Take the time to browse the right gallery and make your decision ahead of time, so you end up with the design you truly want. The good news is that you can find design galleries on the web.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "Tattoo Gallery",
                 "Tattoo Gallery",
                 "Assets/21.jpg",
                 "Finding a great tattoos gallery is essential if you want to find the right design for you. This is not a decision that you should take lightly, and it should certainly not be made on the spot when you're about to enter the artist's chair! Take the time to browse the right gallery and make your decision ahead of time, so you end up with the design you truly want. The good news is that you can find design galleries on the web.",
                 "How Can They Help? Finding a great tattoos gallery is essential if you want to find the right design for you. This is not a decision that you should take lightly, and it should certainly not be made on the spot when you're about to enter the artist's chair! Take the time to browse the right gallery and make your decision ahead of time, so you end up with the design you truly want. The good news is that you can find design galleries on the web. Some of these are free, and some of these are paid. If you pay a small fee, you get access to premium designs that are truly worth your time and effort. They are often arranged into different categories so you can narrow down your focus nearly right away.\n\nSearching through these online galleries is great, because you'll likely find the design that is perfect for you. There are artists from all over the world who submit their designs, and you're sure to find one that just resonates with you. Even if you do find one right away, take some time to really think about it. After all, this is a decision that will be with you for the rest of your life. After you have found the right design, you should print it out and take it to your local tattoo artist. The artist should work with you and ask if there's anything you'd like added or changed on the design. Everything should be done on paper, because sometimes what we verbalize does not happen if the other person doesn't understand! Be sure both you and the tattoo artist know exactly what the result should be. Always remember, if you do find a design that you think is perfect for you, you can add to it, or change something to make the design your own. It's often this personalization that allows you to fall in love with your tattoo. This is something that will always be with you, and you will always show others. It is like a badge of honor because it is something you truly wanted.\n\nIt all starts with finding a tattoos gallery! Once you do this, everything else will fall into place. This is the best way to get excited about your tattoo around. Some of these galleries contain thousands upon thousands of images, so you're sure to find the one that will be perfect for you to make your own.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "Tattoo Design Ideas",
                 "Tattoo Design Ideas",
                 "Assets/22.jpg",
                 "Finding Your Inspiration Has Never Been Easier If you're searching for tattoo design ideas, the chances are good that you are overwhelmed right now. There many different designs to choose from, and maybe you just haven't found the perfect one yet.",
                 "Finding Your Inspiration Has Never Been Easier If you're searching for tattoo design ideas, the chances are good that you are overwhelmed right now. There many different designs to choose from, and maybe you just haven't found the perfect one yet. The problem probably lies within the fact that you aren't looking in the right places. Many people fall into the trap of just using any of the free galleries they find online. While this might seem like a great deal, the designs aren't very unique, and are often of poor quality. There are other ways to find better designs than this. Your first solution might be to visit your local tattoo artist and have a consultation to figure exactly what you want. They are likely to have design books from past ones they have done, as well as examples of what you can get done. Sometimes it's best to talk to the artists because they can make suggestions for you based on the answers you give regarding the design you're looking for.\n\nIt's probably easier, and just as efficient, to join a premium online design gallery. These are filled with thousands of designs from top artists from all over the world. You can really get inspired by searching through the galleries to find exactly what you want. They are broken up into many different categories, with a variety of styles, so you're sure to find the one that's right for you. Some people just use these designs as a jumping off point. They may then take it to their tattoo artist to get it tweaked and add unique style to it. Other people will find exactly what they were looking for amongst the galleries. This makes it easy, because all they need to do is print it out and take it to their artist. However, it's not a huge deal to get a personalized. In fact, some of the online premium Galleries have a way to tweak it right on the screen!\n\nOne of the really neat thing is that some of these galleries are set up as a unique community. You can converse with other people getting a tattoo, and even browse through their personal shots of their tattoos. Sometimes it's a lot easier find something you like among the shots of tattoos that are actually on someone, than it is to browse through the standard pictures! That way you can see what it looks like on someone else and get inspired for your own tattoo. This is truly one of the best ways to find tattoo design ideas. This is a choice that will be with you forever, so make sure you take the time to search.",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "Tattoo Name Designs",
                 "Tattoo Name Designs",
                 "Assets/23.jpg",
                 "Say What You Want! It's important to find the best tattoo name design so you can come out with exactly the look you want. While you might know what lettering you want on your tattoo, that doesn't mean you've settled on a design yet!",
                 "Say What You Want! It's important to find the best tattoo name design so you can come out with exactly the look you want. While you might know what lettering you want on your tattoo, that doesn't mean you've settled on a design yet! There are various ways the lettering can look. You can even incorporate other designs along with it. There's certainly a lot to think about when it comes to name designs. First of all, let's cover the various lettering tattoo types. Some get tribal names, initials, Scripture, scrolls, single words, or other tattoo lettering. There are also many reasons people get specific names tattooed. It might be their own name, the name of their children, the name of a spouse, the name of someone who has passed away, etc.There so many different options, but thankfully there are galleries you can look through to find the right lettering type for you. These galleries serve as inspiration and examples for coloring, font, and overall design. \n\nUnfortunately, there is a lot of junk out there when you search for tattoo galleries online. That's why you want to find some that are designed by true artists. Thankfully, there are excellent galleries you can get access to for just a small fee. This is great, because you won't have to hire your own designer, and everything is done for you. All you'll need to do is print out the example lettering (some even allow you to enter your own text, and it will design it in the lettering of your choice for you) and take it to your local tattoo artist.\n\nIf you want something really unique, you can combine the name tattoo you have chosen with another design. You can further tweak this by telling your tattoo artist exactly what you want. If you're still not inspired, think about the name tattoo designs you've seen on other people. What did you like, and what did you not like? The chances are good that you have seen someone's name design that you really liked, so you just need to add your own style and flair to what is already working for someone else.\n\nThere are a variety of reasons you might want to get Tattoo name designs. There are also many different designs and types of lettering you can choose from. It's important to take the time to find the right one for you, so you can come out with the exact, perfect design you had envisioned in your mind.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "Finding the Right Designs",
                 "Finding the Right Designs",
                 "Assets/24.jpg",
                 "Finding the Right Designs for Tattoos There are a few different ways you can find designs for tattoos. These days, it's easier than ever to find the right designs, with less hassle.",
                 "Finding the Right Designs for Tattoos There are a few different ways you can find designs for tattoos. These days, it's easier than ever to find the right designs, with less hassle. You do need a good selection to choose from, so you end up with something more unique and that really speaks to your personal style. Traditionally, you would just browse through the designs they had at the shop. Sometimes this ends up being more of a snap decision, and something you might not be happy with later on. The good news is that the Internet makes it possible to browse through thousands and thousands of different designs ahead of time, so you can choose one that is right for you. Many artists make their designs available online. Some of these artists have their own websites, and some tattoo artists who work in parlors even make many of their designs available so you can choose one from the comforts of home. You'll also find some free tattoo galleries that have more generic or lower quality designs.\n\nThe best bet for many people will be to pay a small fee to access premium tattoo design galleries. Many of these were created by talented artists from all over the world, so you can find an artist's selection you are really drawn to. From there, you can choose the tattoo so you know is exactly what you want.\n\nSome people will be be drawn to a certain tattoo right away. They'll see it, and immediately know that this is exactly what they want. Other people will have a harder time making a decision. After all, this is something that will stay with you for the rest of your life, so you want to make sure it is the right one. In that case, it's time to narrow your focus.\n\nThe best designs for tattoos galleries are arranged by category. Choose the category that you are thinking of going with. Take note of the designs you'd consider. That way, you can eliminate the distractions of the ones you wouldn't choose anyway, and focus on as you are considering. If there isn't just one that pops out to you, choose the top ones. You can take them in to have a discussion with your tattoo artist to see what they recommend for you. Sometimes just talking about it with another person can help you make the right decision. Finding galleries with designs for tattoos is incredibly easy these days. It is also essential for you to choose the right tattoo to suit the reason you're getting one.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "Tattoo Designs for Women",
                 "Tattoo Designs for Women",
                 "Assets/25.jpg",
                 "Finding Your Perfect Design Specifically finding tattoo designs for women is a lot easier than it used to be. It wasn't that long ago that it was very uncommon for women to get a tattoo at all. These days, it is far more common, and easier than ever to get access to great tattoos designed just for women.",
                 "Finding Your Perfect Design Specifically finding tattoo designs for women is a lot easier than it used to be. It wasn't that long ago that it was very uncommon for women to get a tattoo at all. These days, it is far more common, and easier than ever to get access to great tattoos designed just for women. Many women may not want a tattoo that is strictly for a woman. However, there are many tattoos (such as that of sexy men!) That a woman, and a woman alone, will want to find. In that case, you'll definitely need a top-quality gallery that will have everything you're looking for. As you find the best galleries, you'll notice that they have thousands of designs. Some of these will be applicable to you, and others won't. What's important is that you'll have the largest selection possible so you can find the best to design for you.\n\nMany women tend to get love tattoos, the name of their lover, their children, tribal tattoos, and many more. Simply browse through the different categories and make note of the ones you are considering. Then, you can go through and weed out the ones you don't want after all. You should be left with a few great choices.\n\nIf you're having trouble finding possibilities, get inspiration from your friends. You can even call up your tattoo artist and ask them what is popular with other women in his parlor. He'll probably be more than happy to help you out and give you some suggestions. Some will even do consultations with you to get a feel for your personal style and what you want out of this tattoo. \n\nMost women will find exactly what they want in one of the premium design galleries. Simply blow up the design (the premium Galleries do this for you), print it out, and bring it to your tattoo artist. If you do want anything tweaked, be sure to tell your tattoo artist so the two of you can come up with the exact design you want.\n\nFinding specific tattoo designs for women is a lot easier these days. Take your inspiration from others, and from your own personal style, and work with your tattoo artist so you can rest easy that your tattoo will be perfect in every way. Be careful not to rush into this decision, because it is one that will stay with you for the rest of your life. However, when chosen the right way, the tattoo design you go with will be absolutely perfect.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Tattoo Design Ways",
                 "Tattoo Design Ways",
                 "Assets/26.jpg",
                 "Finding Your Inspiration Has Never Been Easier If you're searching for tattoo design ideas, the chances are good that you are overwhelmed right now.",
                 "Finding Your Inspiration Has Never Been Easier If you're searching for tattoo design ideas, the chances are good that you are overwhelmed right now. There many different designs to choose from, and maybe you just haven't found the perfect one yet. The problem probably lies within the fact that you aren't looking in the right places. Many people fall into the trap of just using any of the free galleries they find online. While this might seem like a great deal, the designs aren't very unique, and are often of poor quality. There are other ways to find better designs than this.\n\nYour first solution might be to visit your local tattoo artist and have a consultation to figure exactly what you want. They are likely to have design books from past ones they have done, as well as examples of what you can get done. Sometimes it's best to talk to the artists because they can make suggestions for you based on the answers you give regarding the design you're looking for. It's probably easier, and just as efficient, to join a premium online design gallery. These are filled with thousands of designs from top artists from all over the world. You can really get inspired by searching through the galleries to find exactly what you want. They are broken up into many different categories, with a variety of styles, so you're sure to find the one that's right for you.\n\nSome people just use these designs as a jumping off point. They may then take it to their tattoo artist to get it tweaked and add unique style to it. Other people will find exactly what they were looking for amongst the galleries. This makes it easy, because all they need to do is print it out and take it to their artist. However, it's not a huge deal to get a personalized. In fact, some of the online premium Galleries have a way to tweak it right on the screen!\n\nOne of the really neat thing is that some of these galleries are set up as a unique community. You can converse with other people getting a tattoo, and even browse through their personal shots of their tattoos. Sometimes it's a lot easier find something you like among the shots of tattoos that are actually on someone, than it is to browse through the standard pictures! That way you can see what it looks like on someone else and get inspired for your own tattoo.\n\nThis is truly one of the best ways to find tattoo design ideas. This is a choice that will be with you forever, so make sure you take the time to search.",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
