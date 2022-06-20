using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EConnect.Psb.Models;

public class FileContent : IDisposable
{
    private const string DefaultFileName = "file";

    public HttpContent Content { get; }
    public string Filename { get; private set; }

    public FileContent(HttpContent content, string? filename = null)
    {
        Content = content;
        EnsureAndSetFilename(filename);
    }

    public FileContent(Stream contents, string? filename = null)
    {
        Content = new StreamContent(contents);
        EnsureAndSetFilename(filename);
    }

    public FileContent(FileStream contents, string? filename = null)
    {
        Content = new StreamContent(contents);
        EnsureAndSetFilename(filename ?? contents.Name);
    }

    public FileContent(byte[] contents, string? filename = null)
    {
        Content = new ByteArrayContent(contents);
        EnsureAndSetFilename(filename);
    }

    public FileContent(string contents, string? filename = null)
    {
        Content = new StringContent(contents);
        EnsureAndSetFilename(filename);
    }

    private void EnsureAndSetFilename(string? filename = null)
    {
        Filename = string.IsNullOrEmpty(filename) ? DefaultFileName : filename;
    }

    private static byte[] CreateWriter(Action<XmlWriter> onWrite)
    {
        // Set writer to exclude Xml & encoding definitions
        var settings = new XmlWriterSettings
        {
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            OmitXmlDeclaration = true,
            Encoding = new UTF8Encoding(false),
            Indent = false
        };

        using var ms = new MemoryStream();
        using (var writer = XmlWriter.Create(ms, settings))
        {
            onWrite(writer);
            writer.Close();
        }

        return ms.ToArray();
    }

    public FileContent(XElement xml, string? filename = null)
        : this(CreateWriter(xml.Save), filename)
    {
    }

    public FileContent(XDocument xml, string? filename = null)
        : this(CreateWriter(xml.Save), filename)
    {
    }

    public FileContent(XmlDocument xml, string? filename = null)
        : this(CreateWriter(xml.Save), filename)
    {
    }

    public void Dispose()
    {
        Content.Dispose();
    }
}