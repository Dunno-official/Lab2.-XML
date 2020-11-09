<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html"></xsl:output>

    <xsl:template match="/">
      <html>
        <body>
          <table border ="1">
            <TR>
              <th>Author</th>
              <th>Title</th>
              <th>Genre</th>
              <th>Price</th>
              <th>Publish year</th>
              <th>Description</th>
            </TR>
            <xsl:for-each select="catalog/book">
            <tr>
              <td>
                <xsl:value-of select="author"/>  
              </td>
              <td>
                <xsl:value-of select="title"/>
              </td>
              <td>
                <xsl:value-of select="genre"/>
              </td>
              <td>
                <xsl:value-of select="price"/>
              </td>
              <td>
                <xsl:value-of select="publishYear"/>
              </td>
              <td>
                <xsl:value-of select="description"/>
              </td>
            </tr>
            </xsl:for-each>
          </table>
        </body>
      </html>
    </xsl:template>
</xsl:stylesheet>
