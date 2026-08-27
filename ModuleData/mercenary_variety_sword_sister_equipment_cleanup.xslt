<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="NPCCharacter[@id='sword_sisters_sister_t3' or @id='sword_sisters_sister_t4' or @id='sword_sisters_sister_t5' or @id='sword_sisters_sister_infantry_t5']/Equipments/EquipmentRoster[not(position() = last())]" />
</xsl:stylesheet>
