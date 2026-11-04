using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Options;
using YDotNet.Document.StickyIndexes;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Maps;
using YDotNet.Document.Types.Texts;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlFragments;
using YDotNet.Document.Types.XmlTexts;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Benchmarks.Memory;

/// <summary>
///     The catalog of operations checked for memory leaks.
/// </summary>
/// <remarks>
///     Operations that only read run against a document created once per scenario. Operations that write create a
///     document per iteration, because a document keeps the history of every change and grows legitimately.
/// </remarks>
public static class Scenarios
{
    /// <summary>
    ///     Gets all scenarios.
    /// </summary>
    public static IReadOnlyList<MemoryScenario> All { get; } =
    [
        .. DocScenarios(),
        .. TextScenarios(),
        .. ArrayScenarios(),
        .. MapScenarios(),
        .. XmlTextScenarios(),
        .. XmlElementScenarios(),
        .. XmlFragmentScenarios(),
        .. BranchScenarios(),
        .. TransactionScenarios(),
        .. UndoManagerScenarios(),
    ];

    /// <summary>
    ///     Gets a scenario that leaks 64 native bytes per iteration on purpose, to check that leaks are detected.
    /// </summary>
    public static MemoryScenario KnownNativeLeak { get; } =
        Repeat("Calibration", "Known native leak", () => System.Runtime.InteropServices.Marshal.AllocHGlobal(cb: 64));

    /// <summary>
    ///     Gets a scenario that keeps 64 bytes of managed objects alive per iteration on purpose, to check that leaks
    ///     are detected.
    /// </summary>
    public static MemoryScenario KnownManagedLeak { get; } =
        new("Calibration", "Known managed leak", _ =>
        {
            var leaked = new List<byte[]>();

            return () => leaked.Add(new byte[40]);
        });

    private static IEnumerable<MemoryScenario> DocScenarios()
    {
        const string group = "Doc";

        yield return Repeat(group, "Create", () => new Doc().Dispose());
        yield return Repeat(group, "Create without dispose", () => _ = new Doc());
        yield return Repeat(
            group,
            "Create with options",
            () => new Doc(new DocOptions { Guid = "guid", CollectionId = "collection", ShouldLoad = true }).Dispose());

        yield return Shared(group, "Read Id", doc => () => _ = doc.Id);
        yield return Shared(group, "Read Guid", doc => () => _ = doc.Guid);
        yield return Shared(
            group,
            "Read CollectionId",
            _ => new Doc(new DocOptions { CollectionId = "collection" }),
            doc => () => _ = doc.CollectionId);
        yield return Shared(group, "Read ShouldLoad", doc => () => _ = doc.ShouldLoad);
        yield return Shared(group, "Read AutoLoad", doc => () => _ = doc.AutoLoad);

        yield return Repeat(
            group,
            "Clear",
            () =>
            {
                using var doc = new Doc();
                doc.Clear();
            });

        yield return Repeat(
            group,
            "Observe clear",
            () =>
            {
                using var doc = new Doc();
                using var subscription = doc.ObserveClear(e => _ = e.Doc);

                doc.Clear();
            });

        yield return Repeat(
            group,
            "Observe updates V1",
            () =>
            {
                using var doc = new Doc();
                using var subscription = doc.ObserveUpdatesV1(e => _ = e.Update);

                InsertText(doc);
            });

        yield return Repeat(
            group,
            "Observe updates V2",
            () =>
            {
                using var doc = new Doc();
                using var subscription = doc.ObserveUpdatesV2(e => _ = e.Update);

                InsertText(doc);
            });

        yield return Repeat(
            group,
            "Observe after transaction",
            () =>
            {
                using var doc = new Doc();
                using var subscription = doc.ObserveAfterTransaction(
                    e =>
                    {
                        _ = e.BeforeState;
                        _ = e.AfterState;
                        _ = e.DeleteSet;
                    });

                InsertText(doc);
            });

        yield return Repeat(
            group,
            "Observe subdocs",
            () =>
            {
                using var doc = new Doc();
                using var subscription = doc.ObserveSubDocs(
                    e =>
                    {
                        _ = e.Added;
                        _ = e.Removed;
                        _ = e.Loaded;
                    });

                var map = doc.Map("map");
                using var subDoc = new Doc();
                using (var input = Input.Doc(subDoc))
                using (var transaction = doc.WriteTransaction())
                {
                    map.Insert(transaction, "sub-doc", input);
                }

                // Removes the sub-document to avoid the leak described in "Map/Insert sub-document".
                using (var transaction = doc.WriteTransaction())
                {
                    map.Remove(transaction, "sub-doc");
                }
            });

        yield return Repeat(
            group,
            "Dispose without unsubscribing",
            () =>
            {
                using var doc = new Doc();
                doc.ObserveUpdatesV1(e => _ = e.Update);
                doc.ObserveAfterTransaction(e => _ = e.AfterState);
                doc.Text("text").Observe(e => _ = e.Delta);

                InsertText(doc);
            });
    }

    private static IEnumerable<MemoryScenario> TextScenarios()
    {
        const string group = "Text";

        yield return Repeat(group, "Create", () =>
        {
            using var doc = new Doc();
            doc.Text("text");
        });

        yield return Repeat(group, "Insert", () =>
        {
            using var doc = new Doc();
            InsertText(doc);
        });

        yield return Repeat(group, "Insert with attributes", () =>
        {
            using var doc = new Doc();
            var text = doc.Text("text");
            using var attributes = Input.Object(new Dictionary<string, Input> { ["bold"] = Input.Boolean(value: true) });

            using var transaction = doc.WriteTransaction();
            text.Insert(transaction, index: 0, "Lucas", attributes);
        });

        yield return Repeat(group, "Insert embed", () =>
        {
            using var doc = new Doc();
            var text = doc.Text("text");
            using var content = Input.Object(new Dictionary<string, Input> { ["image"] = Input.String("url") });

            using var transaction = doc.WriteTransaction();
            text.InsertEmbed(transaction, index: 0, content);
        });

        yield return Repeat(group, "Remove range", () =>
        {
            using var doc = new Doc();
            var text = InsertText(doc);

            using var transaction = doc.WriteTransaction();
            text.RemoveRange(transaction, index: 1, length: 2);
        });

        yield return Repeat(group, "Format", () =>
        {
            using var doc = new Doc();
            var text = InsertText(doc);
            using var attributes = Input.Object(new Dictionary<string, Input> { ["bold"] = Input.Boolean(value: true) });

            using var transaction = doc.WriteTransaction();
            text.Format(transaction, index: 0, length: 3, attributes);
        });

        yield return Shared(group, "Chunks", FormattedTextDoc, doc => Read(doc, t => t.GetText("text")!.Chunks(t)));
        yield return Shared(group, "String", FormattedTextDoc, doc => Read(doc, t => t.GetText("text")!.String(t)));
        yield return Shared(group, "Length", FormattedTextDoc, doc => Read(doc, t => t.GetText("text")!.Length(t)));

        yield return Shared(
            group,
            "Sticky index",
            FormattedTextDoc,
            doc => Write(
                doc,
                t =>
                {
                    var index = t.GetText("text")!.StickyIndex(t, index: 2, StickyAssociationType.After)!;
                    StickyIndex.Decode(index.Encode()).Read(t);
                }));

        yield return Repeat(group, "Observe", () =>
        {
            using var doc = new Doc();
            using var subscription = doc.Text("text").Observe(
                e =>
                {
                    _ = e.Delta;
                    _ = e.Path;
                    _ = e.Target;
                });

            InsertText(doc);
        });
    }

    private static IEnumerable<MemoryScenario> ArrayScenarios()
    {
        const string group = "Array";

        yield return Repeat(group, "Create", () =>
        {
            using var doc = new Doc();
            doc.Array("array");
        });

        yield return Repeat(group, "Insert range", () =>
        {
            using var doc = new Doc();
            InsertArray(doc);
        });

        yield return Repeat(group, "Remove range", () =>
        {
            using var doc = new Doc();
            var array = InsertArray(doc);

            using var transaction = doc.WriteTransaction();
            array.RemoveRange(transaction, index: 0, length: 2);
        });

#pragma warning disable CS0618 // Move is deprecated but still part of the public API.
        yield return Repeat(group, "Move", () =>
        {
            using var doc = new Doc();
            var array = InsertArray(doc);

            using var transaction = doc.WriteTransaction();
            array.Move(transaction, sourceIndex: 0, targetIndex: 2);
        });
#pragma warning restore CS0618

        yield return Shared(group, "Get", ArrayDoc, doc => Read(doc, t => t.GetArray("array")!.Get(t, index: 2)));
        yield return Shared(
            group,
            "Sticky index",
            ArrayDoc,
            doc => Write(doc, t => t.GetArray("array")!.StickyIndex(t, index: 1, StickyAssociationType.After)));
        yield return Shared(group, "Length", ArrayDoc, doc => Read(doc, t => t.GetArray("array")!.Length(t)));
        yield return Shared(
            group,
            "Iterate",
            ArrayDoc,
            doc => Read(
                doc,
                t =>
                {
                    using var iterator = t.GetArray("array")!.Iterate(t);
                    _ = iterator.ToArray();
                }));
        yield return Shared(
            group,
            "Iterate without dispose",
            ArrayDoc,
            doc => Read(doc, t => _ = t.GetArray("array")!.Iterate(t).Take(2).ToArray()));

        yield return Repeat(group, "Observe", () =>
        {
            using var doc = new Doc();
            using var subscription = doc.Array("array").Observe(
                e =>
                {
                    _ = e.Delta;
                    _ = e.Path;
                    _ = e.Target;
                });

            InsertArray(doc);
        });
    }

    private static IEnumerable<MemoryScenario> MapScenarios()
    {
        const string group = "Map";

        yield return Repeat(group, "Create", () =>
        {
            using var doc = new Doc();
            doc.Map("map");
        });

        yield return Repeat(group, "Insert", () =>
        {
            using var doc = new Doc();
            InsertMap(doc);
        });

        yield return Repeat(group, "Insert sub-document", KnownLeaks.SubDocumentCycle, () =>
        {
            using var doc = new Doc();
            var map = doc.Map("map");
            using var subDoc = new Doc();
            using var input = Input.Doc(subDoc);

            using var transaction = doc.WriteTransaction();
            map.Insert(transaction, "sub-doc", input);
        });

        yield return Repeat(group, "Insert sub-document and remove it", () =>
        {
            using var doc = new Doc();
            var map = doc.Map("map");
            using var subDoc = new Doc();
            using (var input = Input.Doc(subDoc))
            using (var transaction = doc.WriteTransaction())
            {
                map.Insert(transaction, "sub-doc", input);
            }

            using (var transaction = doc.WriteTransaction())
            {
                map.Remove(transaction, "sub-doc");
            }
        });

        yield return Shared(group, "Get", MapDoc, doc => Read(doc, t => t.GetMap("map")!.Get(t, "nested")));
        yield return Shared(group, "Get missing key", MapDoc, doc => Read(doc, t => t.GetMap("map")!.Get(t, "missing")));
        yield return Shared(group, "Length", MapDoc, doc => Read(doc, t => t.GetMap("map")!.Length(t)));

        yield return Repeat(group, "Remove", () =>
        {
            using var doc = new Doc();
            var map = InsertMap(doc);

            using var transaction = doc.WriteTransaction();
            map.Remove(transaction, "string");
            map.Remove(transaction, "missing");
        });

        yield return Repeat(group, "Remove all", () =>
        {
            using var doc = new Doc();
            var map = InsertMap(doc);

            using var transaction = doc.WriteTransaction();
            map.RemoveAll(transaction);
        });

        yield return Shared(
            group,
            "Iterate",
            MapDoc,
            doc => Read(
                doc,
                t =>
                {
                    using var iterator = t.GetMap("map")!.Iterate(t);
                    _ = iterator.ToArray();
                }));
        yield return Shared(
            group,
            "Iterate without dispose",
            MapDoc,
            doc => Read(doc, t => _ = t.GetMap("map")!.Iterate(t).Take(1).ToArray()));

        yield return Repeat(group, "Observe", () =>
        {
            using var doc = new Doc();
            using var subscription = doc.Map("map").Observe(
                e =>
                {
                    _ = e.Keys;
                    _ = e.Path;
                    _ = e.Target;
                });

            InsertMap(doc);
        });
    }

    private static IEnumerable<MemoryScenario> XmlTextScenarios()
    {
        const string group = "XmlText";

        yield return Repeat(group, "Create", () =>
        {
            using var doc = new Doc();
            InsertXmlText(doc);
        });

        yield return Repeat(group, "Insert", () =>
        {
            using var doc = new Doc();
            var xmlText = InsertXmlText(doc);
            using var attributes = Input.Object(new Dictionary<string, Input> { ["bold"] = Input.Boolean(value: true) });

            using var transaction = doc.WriteTransaction();
            xmlText.Insert(transaction, index: 0, "Lucas");
            xmlText.Insert(transaction, index: 0, "Viana", attributes);
        });

        yield return Repeat(group, "Remove range", () =>
        {
            using var doc = new Doc();
            var xmlText = InsertXmlText(doc, "Lucas");

            using var transaction = doc.WriteTransaction();
            xmlText.RemoveRange(transaction, index: 1, length: 2);
        });

        yield return Repeat(group, "Format", () =>
        {
            using var doc = new Doc();
            var xmlText = InsertXmlText(doc, "Lucas");
            using var attributes = Input.Object(new Dictionary<string, Input> { ["bold"] = Input.Boolean(value: true) });

            using var transaction = doc.WriteTransaction();
            xmlText.Format(transaction, index: 0, length: 3, attributes);
        });

        yield return Repeat(group, "Insert embed", () =>
        {
            using var doc = new Doc();
            var xmlText = InsertXmlText(doc, "Lucas");
            using var content = Input.Object(new Dictionary<string, Input> { ["image"] = Input.String("url") });

            using var transaction = doc.WriteTransaction();
            xmlText.InsertEmbed(transaction, index: 1, content);
        });

        yield return Repeat(group, "Insert attribute", () =>
        {
            using var doc = new Doc();
            var xmlText = InsertXmlText(doc, "Lucas");

            using var transaction = doc.WriteTransaction();
            xmlText.InsertAttribute(transaction, "bold", "true");
        });

        yield return Repeat(group, "Remove attribute", () =>
        {
            using var doc = new Doc();
            var xmlText = InsertXmlText(doc, "Lucas");

            using var transaction = doc.WriteTransaction();
            xmlText.InsertAttribute(transaction, "bold", "true");
            xmlText.RemoveAttribute(transaction, "bold");
        });

        yield return Shared(group, "Get attribute", XmlDoc, doc => Read(doc, t => XmlTextOf(doc, t).GetAttribute(t, "bold")));
        yield return Shared(
            group,
            "Get missing attribute",
            XmlDoc,
            doc => Read(doc, t => XmlTextOf(doc, t).GetAttribute(t, "missing")));
        yield return Shared(
            group,
            "Iterate attributes",
            XmlDoc,
            doc => Read(
                doc,
                t =>
                {
                    using var iterator = XmlTextOf(doc, t).Iterate(t);
                    _ = iterator.ToArray();
                }));
        yield return Shared(group, "String", XmlDoc, doc => Read(doc, t => XmlTextOf(doc, t).String(t)));
        yield return Shared(group, "Length", XmlDoc, doc => Read(doc, t => XmlTextOf(doc, t).Length(t)));
        yield return Shared(group, "Chunks", XmlDoc, doc => Read(doc, t => XmlTextOf(doc, t).Chunks(t)));
        yield return Shared(group, "Previous sibling", XmlDoc, doc => Read(doc, t => XmlTextOf(doc, t).PreviousSibling(t)));
        yield return Shared(group, "Next sibling", XmlDoc, doc => Read(doc, t => XmlTextOf(doc, t).NextSibling(t)));
        yield return Shared(
            group,
            "Sticky index",
            XmlDoc,
            doc => Write(doc, t => XmlTextOf(doc, t).StickyIndex(t, index: 1, StickyAssociationType.Before)));

        yield return Repeat(group, "Observe", () =>
        {
            using var doc = new Doc();
            var xmlText = InsertXmlText(doc);
            using var subscription = xmlText.Observe(
                e =>
                {
                    _ = e.Delta;
                    _ = e.Keys;
                    _ = e.Target;
                });

            using var transaction = doc.WriteTransaction();
            xmlText.Insert(transaction, index: 0, "Lucas");
            xmlText.InsertAttribute(transaction, "bold", "true");
        });
    }

    private static IEnumerable<MemoryScenario> XmlElementScenarios()
    {
        const string group = "XmlElement";

        yield return Repeat(group, "Create", () =>
        {
            using var doc = new Doc();
            InsertXmlElement(doc);
        });

        yield return Shared(group, "Tag", XmlDoc, doc => Read(doc, t => XmlElementOf(doc, t).Tag(t)));
        yield return Shared(group, "String", XmlDoc, doc => Read(doc, t => XmlElementOf(doc, t).String(t)));

        yield return Repeat(group, "Insert attribute", () =>
        {
            using var doc = new Doc();
            var xmlElement = InsertXmlElement(doc);

            using var transaction = doc.WriteTransaction();
            xmlElement.InsertAttribute(transaction, "color", "red");
        });

        yield return Repeat(group, "Remove attribute", () =>
        {
            using var doc = new Doc();
            var xmlElement = InsertXmlElement(doc);

            using var transaction = doc.WriteTransaction();
            xmlElement.InsertAttribute(transaction, "color", "red");
            xmlElement.RemoveAttribute(transaction, "color");
        });

        yield return Shared(
            group,
            "Get attribute",
            XmlDoc,
            doc => Read(doc, t => XmlElementOf(doc, t).GetAttribute(t, "color")));
        yield return Shared(
            group,
            "Get missing attribute",
            XmlDoc,
            doc => Read(doc, t => XmlElementOf(doc, t).GetAttribute(t, "missing")));
        yield return Shared(
            group,
            "Iterate attributes",
            XmlDoc,
            doc => Read(
                doc,
                t =>
                {
                    using var iterator = XmlElementOf(doc, t).Iterate(t);
                    _ = iterator.ToArray();
                }));
        yield return Shared(
            group,
            "Iterate attributes without dispose",
            XmlDoc,
            doc => Read(doc, t => _ = XmlElementOf(doc, t).Iterate(t).Take(1).ToArray()));
        yield return Shared(group, "Child length", XmlDoc, doc => Read(doc, t => XmlElementOf(doc, t).ChildLength(t)));

        yield return Repeat(group, "Insert text", () =>
        {
            using var doc = new Doc();
            var xmlElement = InsertXmlElement(doc);

            using var transaction = doc.WriteTransaction();
            xmlElement.InsertText(transaction, index: 0);
        });

        yield return Repeat(group, "Insert element", () =>
        {
            using var doc = new Doc();
            var xmlElement = InsertXmlElement(doc);

            using var transaction = doc.WriteTransaction();
            xmlElement.InsertElement(transaction, index: 0, "child");
        });

        yield return Repeat(group, "Remove range", () =>
        {
            using var doc = new Doc();
            var xmlElement = InsertXmlElement(doc);

            using var transaction = doc.WriteTransaction();
            xmlElement.InsertText(transaction, index: 0);
            xmlElement.InsertElement(transaction, index: 1, "child");
            xmlElement.RemoveRange(transaction, index: 0, length: 2);
        });

        yield return Shared(group, "Get", XmlDoc, doc => Read(doc, t => XmlElementOf(doc, t).Get(t, index: 1)));
        yield return Shared(group, "Previous sibling", XmlDoc, doc => Read(doc, t => XmlElementOf(doc, t).PreviousSibling(t)));
        yield return Shared(group, "Next sibling", XmlDoc, doc => Read(doc, t => XmlElementOf(doc, t).NextSibling(t)));
        yield return Shared(group, "Parent", XmlDoc, doc => Read(doc, t => XmlElementOf(doc, t).Parent(t)));
        yield return Shared(group, "First child", XmlDoc, doc => Read(doc, t => XmlElementOf(doc, t).FirstChild(t)));
        yield return Shared(
            group,
            "Tree walker",
            XmlDoc,
            doc => Read(
                doc,
                t =>
                {
                    using var walker = XmlElementOf(doc, t).TreeWalker(t);
                    _ = walker.ToArray();
                }));
        yield return Shared(
            group,
            "Tree walker without dispose",
            XmlDoc,
            doc => Read(doc, t => _ = XmlElementOf(doc, t).TreeWalker(t).Take(1).ToArray()));

        yield return Repeat(group, "Observe", () =>
        {
            using var doc = new Doc();
            var xmlElement = InsertXmlElement(doc);
            using var subscription = xmlElement.Observe(
                e =>
                {
                    _ = e.Delta;
                    _ = e.Keys;
                    _ = e.Path;
                    _ = e.Target;
                });

            using var transaction = doc.WriteTransaction();
            xmlElement.InsertText(transaction, index: 0);
            xmlElement.InsertAttribute(transaction, "color", "red");
        });
    }

    private static IEnumerable<MemoryScenario> XmlFragmentScenarios()
    {
        const string group = "XmlFragment";

        yield return Repeat(group, "Create", () =>
        {
            using var doc = new Doc();
            doc.XmlFragment("xml");
        });

        yield return Shared(group, "Child length", XmlDoc, doc => Read(doc, t => t.GetXmlFragment("xml")!.ChildLength(t)));
        yield return Shared(group, "Get", XmlDoc, doc => Read(doc, t => t.GetXmlFragment("xml")!.Get(t, index: 0)));
        yield return Shared(group, "First child", XmlDoc, doc => Read(doc, t => t.GetXmlFragment("xml")!.FirstChild(t)));
        yield return Shared(
            group,
            "Tree walker",
            XmlDoc,
            doc => Read(
                doc,
                t =>
                {
                    using var walker = t.GetXmlFragment("xml")!.TreeWalker(t);
                    _ = walker.ToArray();
                }));

        yield return Repeat(group, "Remove range", () =>
        {
            using var doc = new Doc();
            var xmlFragment = doc.XmlFragment("xml");

            using var transaction = doc.WriteTransaction();
            xmlFragment.InsertText(transaction, index: 0);
            xmlFragment.InsertElement(transaction, index: 1, "child");
            xmlFragment.RemoveRange(transaction, index: 0, length: 2);
        });

        yield return Repeat(group, "Observe", () =>
        {
            using var doc = new Doc();
            var xmlFragment = doc.XmlFragment("xml");
            using var subscription = xmlFragment.Observe(
                e =>
                {
                    _ = e.Delta;
                    _ = e.Path;
                    _ = e.Target;
                });

            InsertXmlElement(doc);
        });
    }

    private static IEnumerable<MemoryScenario> BranchScenarios()
    {
        const string group = "Branch";

        yield return Shared(
            group,
            "Read transaction",
            XmlDoc,
            doc => () =>
            {
                using var transaction = doc.XmlFragment("xml").ReadTransaction();
            });

        yield return Repeat(group, "Write transaction", () =>
        {
            using var doc = new Doc();
            using var transaction = doc.Text("text").WriteTransaction();
        });

        yield return Repeat(group, "Observe deep", () =>
        {
            using var doc = new Doc();
            var map = doc.Map("map");
            using var subscription = map.ObserveDeep(
                events =>
                {
                    foreach (var e in events)
                    {
                        _ = e.MapEvent.Keys;
                        _ = e.MapEvent.Path;
                    }
                });

            InsertMap(doc);
        });

        yield return Repeat(group, "Observe deep nested", () =>
        {
            using var doc = new Doc();
            var xmlFragment = doc.XmlFragment("xml");
            using var subscription = xmlFragment.ObserveDeep(
                events =>
                {
                    foreach (var e in events)
                    {
                        switch (e.Tag)
                        {
                            case Document.Types.Events.EventBranchTag.XmlElement:
                                _ = e.XmlElementEvent.Keys;
                                _ = e.XmlElementEvent.Delta;
                                _ = e.XmlElementEvent.Path;
                                break;
                            case Document.Types.Events.EventBranchTag.XmlText:
                                _ = e.XmlTextEvent.Keys;
                                _ = e.XmlTextEvent.Delta;
                                break;
                            case Document.Types.Events.EventBranchTag.XmlFragment:
                                _ = e.XmlFragmentEvent.Delta;
                                break;
                        }
                    }
                });

            var xmlElement = InsertXmlElement(doc);

            using var transaction = doc.WriteTransaction();
            xmlElement.InsertAttribute(transaction, "color", "red");
            var xmlText = xmlElement.InsertText(transaction, index: 0);
            xmlText.Insert(transaction, index: 0, "Lucas");
        });
    }

    private static IEnumerable<MemoryScenario> TransactionScenarios()
    {
        const string group = "Transaction";

        yield return Shared(group, "State vector V1", FormattedTextDoc, doc => Read(doc, t => t.StateVectorV1()));
        yield return Shared(group, "State diff V1", FormattedTextDoc, doc => Read(doc, t => t.StateDiffV1(stateVector: null)));
        yield return Shared(group, "State diff V2", FormattedTextDoc, doc => Read(doc, t => t.StateDiffV2(stateVector: null)));
        yield return Shared(group, "Snapshot", FormattedTextDoc, doc => Read(doc, t => t.Snapshot()));
        yield return Shared(group, "Sub docs", XmlDoc, doc => Read(doc, t => t.SubDocs()));
        yield return Shared(group, "Get root types", XmlDoc, doc => Read(
            doc,
            t =>
            {
                t.GetText("text");
                t.GetMap("missing");
                t.GetXmlFragment("xml");
            }));

        yield return Shared(
            group,
            "Apply V1",
            _ => FormattedTextDoc(),
            source =>
            {
                byte[] update;
                using (var transaction = source.ReadTransaction())
                {
                    update = transaction.StateDiffV1(stateVector: null);
                }

                return () =>
                {
                    using var doc = new Doc();
                    using var transaction = doc.WriteTransaction();
                    transaction.ApplyV1(update);
                };
            });
    }

    private static IEnumerable<MemoryScenario> UndoManagerScenarios()
    {
        const string group = "UndoManager";

        yield return Repeat(group, "Undo and redo", () =>
        {
            using var doc = new Doc();
            var text = doc.Text("text");
            using var undoManager = new Document.UndoManagers.UndoManager(doc, text);
            using var added = undoManager.ObserveAdded(e => _ = e.Origin);
            using var popped = undoManager.ObservePopped(e => _ = e.Origin);

            InsertText(doc);
            undoManager.Undo();
            undoManager.Redo();
        });
    }

    private static MemoryScenario Repeat(string group, string name, Action action)
    {
        return new MemoryScenario(group, name, _ => action);
    }

    private static MemoryScenario Repeat(string group, string name, string knownLeak, Action action)
    {
        return new MemoryScenario(group, name, _ => action) { KnownLeak = knownLeak };
    }

    private static MemoryScenario Shared(string group, string name, Func<Doc, Action> operation)
    {
        return Shared(group, name, _ => new Doc(), operation);
    }

    private static MemoryScenario Shared(string group, string name, Func<Doc> createDoc, Func<Doc, Action> operation)
    {
        return Shared(group, name, _ => createDoc(), operation);
    }

    private static MemoryScenario Shared(
        string group,
        string name,
        Func<ScenarioContext, Doc> createDoc,
        Func<Doc, Action> operation)
    {
        return new MemoryScenario(group, name, context => operation(context.Track(createDoc(context))));
    }

    private static Action Read<T>(Doc doc, Func<Transaction, T> read)
    {
        return () =>
        {
            using var transaction = doc.ReadTransaction();
            _ = read(transaction);
        };
    }

    private static Action Read(Doc doc, Action<Transaction> read)
    {
        return () =>
        {
            using var transaction = doc.ReadTransaction();
            read(transaction);
        };
    }

    // For operations that do not change the document but still require a read-write transaction.
    private static Action Write<T>(Doc doc, Func<Transaction, T> operation)
    {
        return () =>
        {
            using var transaction = doc.WriteTransaction();
            _ = operation(transaction);
        };
    }

    private static Action Write(Doc doc, Action<Transaction> operation)
    {
        return () =>
        {
            using var transaction = doc.WriteTransaction();
            operation(transaction);
        };
    }

    private static Text InsertText(Doc doc)
    {
        var text = doc.Text("text");

        using var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas Viana");

        return text;
    }

    private static Array InsertArray(Doc doc)
    {
        var array = doc.Array("array");
        using var inputs = new InputList(
            Input.String("Lucas"),
            Input.Long(value: 42),
            Input.Map(new Dictionary<string, Input> { ["key"] = Input.String("value") }),
            Input.Array([Input.Boolean(value: true), Input.Null()]));

        using var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, inputs.Items);

        return array;
    }

    private static Map InsertMap(Doc doc)
    {
        var map = doc.Map("map");
        using var inputs = new InputList(
            Input.String("Lucas"),
            Input.Double(value: 1.5),
            Input.Map(new Dictionary<string, Input> { ["key"] = Input.String("value") }),
            Input.Object(new Dictionary<string, Input> { ["key"] = Input.Bytes([1, 2, 3]) }));

        using var transaction = doc.WriteTransaction();
        map.Insert(transaction, "string", inputs.Items[0]);
        map.Insert(transaction, "double", inputs.Items[1]);
        map.Insert(transaction, "nested", inputs.Items[2]);
        map.Insert(transaction, "object", inputs.Items[3]);

        return map;
    }

    private static XmlText InsertXmlText(Doc doc, string? value = null)
    {
        var xmlFragment = doc.XmlFragment("xml");

        using var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);

        if (value != null)
        {
            xmlText.Insert(transaction, index: 0, value);
        }

        return xmlText;
    }

    private static XmlElement InsertXmlElement(Doc doc)
    {
        var xmlFragment = doc.XmlFragment("xml");

        using var transaction = doc.WriteTransaction();
        return xmlFragment.InsertElement(transaction, index: 0, "element");
    }

    private static Doc FormattedTextDoc()
    {
        var doc = new Doc();
        var text = doc.Text("text");
        using var bold = Input.Object(new Dictionary<string, Input> { ["bold"] = Input.Boolean(value: true) });
        using var embed = Input.Object(new Dictionary<string, Input> { ["image"] = Input.String("url") });

        using var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas Viana");
        text.Format(transaction, index: 0, length: 5, bold);
        text.InsertEmbed(transaction, index: 5, embed);

        return doc;
    }

    private static Doc ArrayDoc()
    {
        var doc = new Doc();
        InsertArray(doc);

        return doc;
    }

    private static Doc MapDoc()
    {
        var doc = new Doc();
        InsertMap(doc);

        return doc;
    }

    /// <summary>
    ///     Creates a fragment with <c>[text, element, text]</c>. The element has an attribute, a text child and an
    ///     element child, and the first text has formatted content and an attribute.
    /// </summary>
    private static Doc XmlDoc()
    {
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml");
        using var bold = Input.Object(new Dictionary<string, Input> { ["bold"] = Input.Boolean(value: true) });

        using var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        xmlText.Insert(transaction, index: 0, "Lucas Viana", bold);
        xmlText.InsertAttribute(transaction, "bold", "true");

        var xmlElement = xmlFragment.InsertElement(transaction, index: 1, "element");
        xmlElement.InsertAttribute(transaction, "color", "red");
        xmlElement.InsertText(transaction, index: 0).Insert(transaction, index: 0, "child");
        xmlElement.InsertElement(transaction, index: 1, "child");

        xmlFragment.InsertText(transaction, index: 2);

        return doc;
    }

    private static XmlText XmlTextOf(Doc doc, Transaction transaction)
    {
        return transaction.GetXmlFragment("xml")!.Get(transaction, index: 0)!.XmlText;
    }

    private static XmlElement XmlElementOf(Doc doc, Transaction transaction)
    {
        return transaction.GetXmlFragment("xml")!.Get(transaction, index: 1)!.XmlElement;
    }

    /// <summary>
    ///     Leaks that cannot be fixed in YDotNet, with the reason.
    /// </summary>
    public static class KnownLeaks
    {
        /// <summary>
        ///     Yrs stores a strong reference to the parent document inside the item that holds a sub-document, so a
        ///     document that still contains a sub-document is never released. Removing the sub-document before
        ///     disposing the parent document avoids it. See https://github.com/y-crdt/y-crdt/issues/585.
        /// </summary>
        public const string SubDocumentCycle =
            "Yrs keeps a reference cycle between a document and the items holding its sub-documents (y-crdt/y-crdt#585).";
    }

    private sealed class InputList(params Input[] items) : IDisposable
    {
        public Input[] Items { get; } = items;

        public void Dispose()
        {
            foreach (var item in Items)
            {
                item.Dispose();
            }
        }
    }
}
