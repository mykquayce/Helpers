<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">
    <xsl:for-each select="/cinemas/cinema/films/film[not(.=preceding::*)]">
      <Film>
        <Edi>
          <xsl:value-of select="@edi"/>
        </Edi>
        <Title>
          <xsl:value-of select="@title"/>
        </Title>
        <Length>
          <xsl:value-of select="substring-before(@length, ' ')"/>
        </Length>
      </Film>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>
