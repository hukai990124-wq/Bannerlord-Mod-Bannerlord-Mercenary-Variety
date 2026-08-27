<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes" indent="yes" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="NPCCharacter[@id='sea_raiders_chief']/upgrade_targets">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
      <upgrade_target id="NPCCharacter.sea_raiders_boss" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="NPCCharacter[@id='sea_raiders_boss']/upgrade_targets">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
      <upgrade_target id="NPCCharacter.mv_sea_raider_warlord" />
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
