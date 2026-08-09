<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Culture[@id='empire']/basic_mercenary_troops">
    <basic_mercenary_troops>
      <template name="NPCCharacter.western_mercenary" />
      <template name="NPCCharacter.sword_sisters_sister_t3" />
    </basic_mercenary_troops>
  </xsl:template>

  <xsl:template match="Culture[@id='sturgia']/basic_mercenary_troops">
    <basic_mercenary_troops>
      <template name="NPCCharacter.mv_old_vaegir_recruit" />
      <template name="NPCCharacter.sword_sisters_sister_t3" />
    </basic_mercenary_troops>
  </xsl:template>

  <xsl:template match="Culture[@id='nord']">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()[not(self::basic_mercenary_troops)]" />
      <basic_mercenary_troops>
        <template name="NPCCharacter.mv_old_vaegir_recruit" />
        <template name="NPCCharacter.mv_valkyrie_shield_maiden_t4" />
      </basic_mercenary_troops>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
