using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EConnect.Psb.Models;

public class FileContent : IDisposable
{
    public HttpContent Content { get; }
    public string? Filename { get; set; }

    public FileContent(HttpContent content, string? filename = null)
    {
        Content = content;
        Filename = filename;
    }

    public FileContent(Stream contents, string? filename = null)
    {
        Content = new StreamContent(contents);
        Filename = filename;
    }

    public FileContent(FileStream contents)
    {
        Content = new StreamContent(contents);
        Filename = contents.Name;
    }

    public FileContent(byte[] contents, string? filename = null)
    {
        Content = new ByteArrayContent(contents);
        Filename = filename;
    }

    public FileContent(string contents, string? filename = null)
    {
        Content = new StringContent(contents);
        Filename = filename;
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

    public FileContent(XElement xml) : this(CreateWriter(xml.Save))
    {
    }

    public FileContent(XDocument xml) : this(CreateWriter(xml.Save))
    {
    }

    public FileContent(XmlDocument xml) : this(CreateWriter(xml.Save))
    {
    }

    public static implicit operator FileContent(Stream contents)
    {
        return new FileContent(contents);
    }

    public static implicit operator FileContent(FileStream contents)
    {
        return new FileContent(contents);
    }

    public static implicit operator FileContent(byte[] contents)
    {
        return new FileContent(contents);
    }

    public static implicit operator FileContent(string contents)
    {
        return new FileContent(contents);
    }

    public static implicit operator FileContent(XElement contents)
    {
        return new FileContent(contents);
    }

    public static implicit operator FileContent(XDocument contents)
    {
        return new FileContent(contents);
    }

    public static implicit operator FileContent(XmlDocument contents)
    {
        return new FileContent(contents);
    }

    public void Dispose()
    {
        Content.Dispose();
    }
}