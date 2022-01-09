using Domain.Repository;
using Hubs;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Model.Configuration;
using Model.Entity;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using IClientProxy = Microsoft.AspNetCore.SignalR.IClientProxy;

namespace ChatTest {
    public class Tests {
        /* Create a context in a test database */
        public ChatDbContext CreateContext() {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "ChatTestDatabase")
                .Options;
            return new ChatDbContext(options);
        }

        /*
         * User muss sich registrieren können.
         * Nachdem ein User registriert wurde, wird ihm eine ID zugeteilt, diese wird im Test verglichen.
         */
        [Fact]
        public async Task UserRegisterTest() {
            using(var context = CreateContext()) {
                UserRepository repository = new UserRepository(context);

                User user = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                await repository.CreateAsync(user);

                Assert.True(user.Id > 0);
            }
        }

        /*
         * User soll eine E-Mail, zur Identität Bestätigung bekommen.
         * Nachdem ein User registriert wurde, wird eine Aktivierungscode erstellt. Es wird geprüft, ob ein Aktivierungscode für den Benutzer da ist.
         */
        [Fact]
        public async Task UserEmailVerificationTest() {
            using(var context = CreateContext()) {
                UserRepository repository = new UserRepository(context);

                // User wird erstellt
                User user = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                user = await repository.CreateAsync(user);

                // Code wird erstellt
                var code = await repository.CreateEmailVerificationCode(user);

                Assert.NotNull(code);
            }
        }

        /*
         * Anmeldung soll auf dem Gerät gespeichert werden.
         * Nachdem sich ein Benutzer anmeldet, muss nach dem Neustart der gleiche Benutzer angemeldet sein.
         */

        /*
         * Password-Wiederherstellung soll vorhanden sein.
         * Nachdem ein User die Wiederherstellung aufruft, wird eine Aktivierungscode erstellt. Es wird geprüft,ob ein Aktivierungscode für den Benutzer da ist.
         */
        [Fact]
        public async Task UserPasswordResetTest() {
            using(var context = CreateContext()) {
                UserRepository repository = new UserRepository(context);

                // User wird erstellen
                User user = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                user = await repository.CreateAsync(user);

                // Code wird erstellt
                var code = await repository.CreatePasswordResetCode(user);

                Assert.NotNull(code);
            }
        }

        /*
         * Freundschaftsanfragen müssen möglich sein.
         * Es wird von „User A“ eine Freundschaftsanfragean „User B“ geschickt. Es wird überprüft,ob eine Freundschaftsanfrage für „User B“ an „User A“ da ist.
         */
        [Fact]
        public async Task FriendRequestTest() {
            using(var context = CreateContext()) {
                // User werden erstellt
                UserRepository repository = new UserRepository(context);

                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                User userB = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userB = await repository.CreateAsync(userB);

                // Freundschaftsanfrage wird geschickt
                await repository.AddFriend(userA, userB);

                // Freundschaftsliste wird aufgerufen
                var fr = await repository.GetFriendRequests(userB);

                Assert.True(fr[0].Username == userA.Username);
            }
        }

        /*
         * Man muss mit einem anderen User chatten können.
         * Es wird getestet, ob „User B“ mit „User A“ in einem Chat ist.
         */
        [Fact]
        public async Task UserChatTest() {
            using(var context = CreateContext()) {
                // User werden erstellt
                UserRepository repository = new UserRepository(context);
                ChatRepository chatRepository = new ChatRepository(context);
                MessageRepository messageRepository = new MessageRepository(context);

                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                User userB = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userB = await repository.CreateAsync(userB);

                // Chat wird erstellt
                var sc = await chatRepository.CreateSinglechat(userA, userB);

                // Nachricht wird gesendet
                await messageRepository.SendMessage(new Message() {
                    Chat = sc,
                    User = userA,
                    Content = "test"
                });

                // Nachrichten werden geladen
                var messages = await messageRepository.GetMessages(sc);

                // User von einem Chat werden geladen
                var users = await chatRepository.GetChatUsers(sc);

                Assert.True(messages[0].Content.Decrypt() == "test");
                Assert.True(users.Count == 2);
            }
        }

        /*
         * Einstellung wer dir schreiben kann (nur Freunde oder jeder).
         * Wenn „User A“ auf öffentlich gestellt ist, soll „User B“ ihm ohne Freundschaftsanfrage eine Nachricht senden können.
         * -> Wird über Blazor überprüft, nicht über das Repo.
         */

        /*
         * Nachrichten sollen löschbar sein.
         * Wenn „User A“ eine Nachricht löscht, soll sie „User B“ nicht mehr finden.
         */
        [Fact]
        public async Task DeleteMessageTest() {
            using(var context = CreateContext()) {
                UserRepository repository = new UserRepository(context);
                ChatRepository chatRepository = new ChatRepository(context);
                MessageRepository messageRepository = new MessageRepository(context);

                // User werden erstellt
                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                User userB = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userB = await repository.CreateAsync(userB);

                // Chat wird erstellt
                var sc = await chatRepository.CreateSinglechat(userA, userB);

                // Nachricht wird gesendet
                Message msg = new Message() {
                    Chat = sc,
                    User = userA,
                    Content = "test"
                };

                msg = await messageRepository.SendMessage(msg);

                // Nachricht wird gelöscht
                await messageRepository.DeleteMessage(msg);

                // Nachrichten werden geladen
                var messages = await messageRepository.GetMessages(sc);

                Assert.True(messages.Count == 0);
            }
        }

        /*
         * Die Nachrichten sollen in Real-time übertragen werden.
         * Wenn „User A“ eine Nachricht an „User B“ schickt, soll sie innerhalb von einem bestimmten Zeit-Intervall von selbst ankommen.
         */
        [Fact]
        public async Task RealtimeMessageTest() {
            using(var context = CreateContext()) {
                Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
                Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();

                mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

                ChatHub hub = new ChatHub() {
                    Clients = mockClients.Object
                };
                UserRepository repository = new UserRepository(context);
                ChatRepository chatRepository = new ChatRepository(context);
                MessageRepository messageRepository = new MessageRepository(context);

                // User werden erstellt
                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                User userB = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userB = await repository.CreateAsync(userB);

                // Chat wird erstellt
                var sc = await chatRepository.CreateSinglechat(userA, userB);

                // Nachricht wird gesendet
                Message msg = new Message() {
                    Chat = sc,
                    User = userA,
                    Content = "test"
                };

                msg = await messageRepository.SendMessage(msg);

                // Send Signal
                await hub.SendMessage(sc.Id, msg.MessageId);

                // Receive Signal
                mockClients.Verify(clients => clients.All, Times.Once);
            }
        }

        /*
         * Man soll Gruppenchats erstellen können.
         * Es sollen „User A“, „User B“ und „User C“ in einem Gruppenchat sein.
         */
        [Fact]
        public async Task GroupchatTest() {
            using(var context = CreateContext()) {
                UserRepository repository = new UserRepository(context);
                ChatRepository chatRepository = new ChatRepository(context);
                MessageRepository messageRepository = new MessageRepository(context);

                // User werden erstellt
                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                User userB = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userB = await repository.CreateAsync(userB);

                User userC = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userC = await repository.CreateAsync(userC);

                // Chat wird erstellt
                Groupchat gc = new Groupchat() {
                    Name = "test",
                    Users = new [] {
                        new ChatUser { User = userA },
                        new ChatUser { User = userB },
                        new ChatUser { User = userC }
                    }
                };

                gc = await chatRepository.CreateGroupchat(gc);

                Assert.True(gc.Users.Count == 3);
            }
        }

        /*
         * Es soll eine Profilanpassung möglich sein (Profilbild, Benutzername).
         * Wenn „User A“ Foo heißt, soll er danach Bar heißen.
         */
        [Fact]
        public async Task UserEditTest() {
            using(var context = CreateContext()) {
                UserRepository repository = new UserRepository(context);

                // User wird erstellt
                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                // Daten werden geändert
                userA.Username = "cat";

                await repository.UpdateAsync(userA);

                // Daten werden aufgerufen
                var result = await repository.ReadAsync(userA.Id);

                Assert.True(result.Username == "cat");
            }
        }

        /*
         * Einen Light-und Darkmodus haben.
         * Nur Zustand als bool testbar.
         */
        [Fact]
        public async Task DarkmodeTest() {
            using(var context = CreateContext()) {
                UserRepository repository = new UserRepository(context);

                // User werden erstellt
                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                // Daten werden geändert
                userA.DarkMode = true;

                await repository.UpdateAsync(userA);

                // Daten werden aufgerufen
                var result = await repository.ReadAsync(userA.Id);

                Assert.True(result.DarkMode);
            }
        }

        /*
         * Es können Bilder und Dateien an eine Nachricht angehängt werden.
         * Testen ob Dateien an einer Nachricht angehängt sind.
         */
        [Fact]
        public async Task MessageAttachmentTest() {
            using(var context = CreateContext()) {
                // User werden erstellt
                UserRepository repository = new UserRepository(context);
                ChatRepository chatRepository = new ChatRepository(context);
                MessageRepository messageRepository = new MessageRepository(context);

                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                User userB = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userB = await repository.CreateAsync(userB);

                // Chat wird erstellt
                var sc = await chatRepository.CreateSinglechat(userA, userB);

                // Nachricht wird gesendet
                Message msg = new Message() {
                    Chat = sc,
                    User = userA,
                    Content = "test",
                    MessageAttachments = new[] {
                        new MessageAttachment() {
                            Name = "file",
                            Attachment = new EFCAT.Model.Annotation.Document() {
                                Content = new byte[0],
                                Type = "file"
                            }
                        }
                    }
                };

                msg = await messageRepository.SendMessage(msg);


                // Nachrichten lesen
                var result = await messageRepository.GetMessages(sc);

                Assert.True(result[0].MessageAttachments.Count > 0);
            }
        }

        /*
         * Man sieht, ob eine Nachricht gelesen wurde.
         * Wenn „User A“ in den „Chat A“ schaut, werden alle Nachrichten als gelesen bemerkt.
         */
        [Fact]
        public async Task MessageReadTest() {
            using(var context = CreateContext()) {
                // User werden erstellt
                UserRepository repository = new UserRepository(context);
                ChatRepository chatRepository = new ChatRepository(context);
                MessageRepository messageRepository = new MessageRepository(context);

                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                User userB = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userB = await repository.CreateAsync(userB);

                // Chat wird erstellt
                var sc = await chatRepository.CreateSinglechat(userA, userB);

                // Nachricht wird gesendet
                Message msg = new Message() {
                    Chat = sc,
                    User = userA,
                    Content = "test"
                };

                msg = await messageRepository.SendMessage(msg);
                

                // Nachricht von UserA gelesen
                await messageRepository.SetMessageStatus(userA, msg);
                var resultA = await messageRepository.CheckMessageStatus(msg);

                // Nachricht von UserB gelesen
                await messageRepository.SetMessageStatus(userB, msg);
                var resultB = await messageRepository.CheckMessageStatus(msg);

                Assert.True(resultA.Count == 1);
                Assert.True(resultB.Count == 2);
            }
        }

        /*
         * Nachrichten sollen beim Versenden verschlüsselt werden.
         * Wenn eine Nachricht verschickt wird, soll sie nicht gleich ausschauen wie das gespeicherte Ergebnis.
         */
        [Fact]
        public async Task MessageCryptTest() {
            using(var context = CreateContext()) {
                // User werden erstellt
                UserRepository repository = new UserRepository(context);
                ChatRepository chatRepository = new ChatRepository(context);
                MessageRepository messageRepository = new MessageRepository(context);

                User userA = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userA = await repository.CreateAsync(userA);

                User userB = new User() {
                    Username = "Test",
                    Email = "test@mail.com",
                    Password = "test"
                };

                userB = await repository.CreateAsync(userB);

                // Chat wird erstellt
                var sc = await chatRepository.CreateSinglechat(userA, userB);

                // Nachricht wird gesendet
                await messageRepository.SendMessage(new Message() {
                    Chat = sc,
                    User = userA,
                    Content = "test"
                });

                // Nachrichten werden geladen
                var messages = await messageRepository.GetMessages(sc);

                Assert.False(messages[0].Content.ToString() == "test");
            }
        }
    }
}