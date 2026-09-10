<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="NPCCharacter[@id='brotherhood_of_woods_tier_3']/upgrade_targets">
    <upgrade_targets>
      <upgrade_target id="NPCCharacter.mv_brotherhood_of_woods_tier_4" />
    </upgrade_targets>
  </xsl:template>
</xsl:stylesheet>
